using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore.Internal;
using Renci.SshNet;

namespace BTCPayServer.Installer
{
    public class InstallerHub : Hub
    {
        private readonly InstallerService _installerService;

        public InstallerHub(InstallerService installerService)
        {
            _installerService = installerService;
        }

        public const string Path = "/api/v1/installer/hub";

        public async Task<bool> TestSshDetails(SetSSHDetailsRequest request)
        {
            return await _installerService.TestSshDetails(request);
        }

        public async Task<string> CheckSpace(SetSSHDetailsRequest request)
        {
            return await _installerService.CheckSpace(request);
        }        
        public async Task<string> CheckDocker(SetSSHDetailsRequest request)
        {
            return await _installerService.CheckDocker(request);
        }

        public async Task<bool> CheckHost(CheckHostRequest request)
        {
            return await _installerService.CheckHost(request);
        }

        public async Task<ChannelReader<string>> Install(InstallRequest request)
        {
            var channel = Channel.CreateUnbounded<string>();
            _ = _installerService.Install(request, channel.Writer);
            return channel.Reader;
        }
    }

    public class InstallerService
    {
        public async Task<bool> TestSshDetails(SetSSHDetailsRequest request)
        {
            try
            {
                var client = await Connect(request);
                if (client == null)
                {
                    return false;
                }

                await Disconnect(client);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<SshClient> Connect(SetSSHDetailsRequest request)
        {
            try
            {
                var client = new SshClient(request.Host, request.Username, request.Password);
                client.Connect();
                client.KeepAliveInterval = TimeSpan.FromSeconds(5);
                var id = Guid.NewGuid().ToString();
                return client;
            }
            catch (Exception)
            {
                return null;
            }
        }


        public async Task<bool> Disconnect(SshClient client)
        {
            try
            {
                client.Disconnect();
                client.Dispose();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<string> CheckSpace(SetSSHDetailsRequest request)
        {
            return await RunCommand(request, "df -H");
        }
        public async Task<string> CheckDocker(SetSSHDetailsRequest request)
        {
            return await RunCommand(request, "docker ps");
        }

        public async Task<string> RunCommand(SetSSHDetailsRequest request, string command)
        {
            var client = await Connect(request);
            var cmd = client?.RunCommand(command);
            return string.IsNullOrEmpty(cmd?.Result)? cmd?.Error : cmd?.Result;
        }

        public async Task<bool> CheckHost(CheckHostRequest request)
        {
            var basicCheck = Uri.CheckHostName(request.Check);
            if (basicCheck != UriHostNameType.Dns && basicCheck != UriHostNameType.Unknown)
            {
                return false;
            }

            var vpsType = Uri.CheckHostName(request.VpsHost);
            if (vpsType == UriHostNameType.Dns)
            {
                var vpsIps = Dns.GetHostAddresses(request.VpsHost);
                return Dns.GetHostAddresses(request.Check).ToList().Any(address =>
                    vpsIps.Any(ipAddress =>
                        ipAddress.Equals(address)));
            }

            return Dns.GetHostAddresses(request.Check).ToList().Any(address =>
                address.ToString().Equals(request.VpsHost, StringComparison.InvariantCultureIgnoreCase));
        }

        public async Task Install(InstallRequest installRequest, ChannelWriter<string> writer)
        {
            var client = await Connect(installRequest);

            try
            {
                if (string.IsNullOrEmpty(installRequest.DockerBranch))
                {
                    installRequest.DockerBranch = "master";
                }

                if (string.IsNullOrEmpty(installRequest.DockerRepositoryUrl))
                {
                    installRequest.DockerRepositoryUrl = "https://github.com/btcpayserver/btcpayserver-docker";
                }

                await SendCommandToStream(writer, client, "apt-get update && apt-get install -y git");
                await SendCommandToStream(writer, client,
                    "[ -d \"btcpayserver-docker\" ] && rm -r btcpayserver-docker && echo \"Deleted existing folder\"",
                    true);
                await SendCommandToStream(writer, client,
                    $"git clone {installRequest.DockerRepositoryUrl} btcpayserver-docker -b {installRequest.DockerBranch} ");


                List<string> commands = new List<string>();
                StringBuilder sb = new StringBuilder();

                for (var i = 0; i < installRequest.Coins.Count(); i++)
                {
                    commands.Add($"export BTCPAYGEN_CRYPTO{i + 1}=\"{installRequest.Coins[i]}\"");
                }

                commands.Add($"export NBITCOIN_NETWORK=\"{installRequest.Network}\"");
                commands.Add($"export BTCPAY_HOST=\"{installRequest.BtcpayHost}\"");
                commands.Add(
                    $"export BTCPAYGEN_REVERSEPROXY=\"{installRequest.ReverseProxy}\"");
                commands.Add($"export BTCPAYGEN_LIGHTNING=\"{installRequest.Lightning}\"");

                commands.Add(
                    $"export LIGHTNING_ALIAS=\"{installRequest.LightningAlias}\"");
                commands.Add(
                    $"export LETSENCRYPT_EMAIL=\"{installRequest.LetsEncryptEmail}\"");
                commands.Add(
                    $"export BTCPAYGEN_DOCKER_IMAGE=\"{installRequest.BtcpayGeneratorDockerImage}\"");
                commands.Add(
                    $"export BTCPAY_IMAGE=\"{installRequest.BtcPayDockerImage}\"");
                commands.Add(
                    $"export LIBREPATRON_HOST=\"{installRequest.LibrePatronHost}\"");
                commands.Add(
                    $"export WOOCOMMERCE_HOST=\"{installRequest.WoocommerceHost}\"");
                commands.Add(
                    $"export BTCTRANSMUTER_HOST=\"{installRequest.BtcTransmuterHost}\"");
                commands.Add(
                    $"export BTCTRANSMUTER_CRYPTOS=\"{string.Join(',',installRequest.Coins)}\"");
                commands.Add(
                    $"export BTCPAYGEN_ADDITIONAL_FRAGMENTS=\"{string.Join(';', installRequest.AdditionalFragments)}\"");
                commands.Add(
                    $"export ACME_CA_URI=\"https://acme-v01.api.letsencrypt.org/directory\"");
                commands.Add(
                    $"ssh-keygen -t rsa -f /root/.ssh/id_rsa_btcpay -q -P \"\"");
                commands.Add(
                    $"echo \"# Key used by BTCPay Server\" >> /root/.ssh/authorized_keys");
                commands.Add(
                    $"cat /root/.ssh/id_rsa_btcpay.pub >> /root/.ssh/authorized_keys");
                commands.Add(
                    $"export BTCPAY_HOST_SSHKEYFILE=/root/.ssh/id_rsa_btcpay");
                
                commands.Add($"cd  btcpayserver-docker && . ./btcpay-setup.sh -i");

                await SendCommandToStream(writer, client, sb.AppendJoin($" && ", commands).ToString());


//                
//                
//                for (var i = 0; i < installRequest.Coins.Count(); i++)
//                {
//                    await SendCommandToStream(writer, client,
//                        $"export BTCPAYGEN_CRYPTO{i + 1}=\"{installRequest.Coins[i]}\"");
//                }
//
//                await SendCommandToStream(writer, client, $"export NBITCOIN_NETWORK=\"{installRequest.Network}\"");
//                await SendCommandToStream(writer, client, $"export BTCPAY_HOST=\"{installRequest.BtcpayHost}\"");
//                await SendCommandToStream(writer, client,
//                    $"export BTCPAYGEN_REVERSEPROXY=\"{installRequest.ReverseProxy}\"");
//                await SendCommandToStream(writer, client, $"export BTCPAYGEN_LIGHTNING=\"{installRequest.Lightning}\"");
//
//                await SendCommandToStream(writer, client,
//                    $"export LIGHTNING_ALIAS=\"{installRequest.LightningAlias}\"");
//                await SendCommandToStream(writer, client,
//                    $"export LETSENCRYPT_EMAIL=\"{installRequest.LetsEncryptEmail}\"");
//                await SendCommandToStream(writer, client,
//                    $"export BTCPAYGEN_DOCKER_IMAGE=\"{installRequest.BtcpayGeneratorDockerImage}\"");
//                await SendCommandToStream(writer, client,
//                    $"export BTCPAY_IMAGE=\"{installRequest.BtcPayDockerImage}\"");
//                await SendCommandToStream(writer, client,
//                    $"export LIBREPATRON_HOST=\"{installRequest.LibrePatronHost}\"");
//                await SendCommandToStream(writer, client,
//                    $"export WOOCOMMERCE_HOST=\"{installRequest.WoocommerceHost}\"");
//                await SendCommandToStream(writer, client,
//                    $"export BTCPAYGEN_ADDITIONAL_FRAGMENTS=\"{string.Join(';', installRequest.AdditionalFragments)}\"");
//                await SendCommandToStream(writer, client,
//                    $"export ACME_CA_URI=\"https://acme-v01.api.letsencrypt.org/directory\"");
//                await SendCommandToStream(writer, client, $"ssh-keygen -t rsa -f /root/.ssh/id_rsa_btcpay -q -P \"\" -y");
//                await SendCommandToStream(writer, client,
//                    $"echo \"# Key used by BTCPay Server\" >> /root/.ssh/authorized_keys");
//                await SendCommandToStream(writer, client,
//                    $"cat /root/.ssh/id_rsa_btcpay.pub >> /root/.ssh/authorized_keys");
//                await SendCommandToStream(writer, client, $"export BTCPAY_HOST_SSHKEYFILE=/root/.ssh/id_rsa_btcpay");
//                await SendCommandToStream(writer, client, $"cd  btcpayserver-docker && . ./btcpay-setup.sh -i");


                writer.Complete();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);

                writer.Complete(e);
            }
            finally
            {
                await Disconnect(client);
            }
        }

        private async Task SendCommandToStream(ChannelWriter<string> writer, SshClient stream, string command,
            bool ignoreExitStatus = false)
        {
            if (!stream.IsConnected)
            {
                stream.Connect();
            }

            Console.WriteLine($"await writer.WriteAsync(\"{command}\")");
            await writer.WriteAsync(command);
            var cmd = stream.CreateCommand(command);

            Console.WriteLine($"cmd.Execute();");
            var result = cmd.Execute();

            Console.WriteLine($"cmd.ExitStatus {cmd.ExitStatus}");
            if (!ignoreExitStatus && cmd.ExitStatus > 0)
            {
                await writer.WriteAsync("###STOPPING Error:\n" + cmd.Error);
                throw new Exception(cmd.Error);
            }

            if (!string.IsNullOrEmpty(cmd.Error))
            {
                await writer.WriteAsync("###Error:\n" + cmd.Error);
            }

            Console.WriteLine($"writer.WriteAsync(\"{result}\")");
            await writer.WriteAsync(result);
        }
    }

    public class InstallRequest : SetSSHDetailsRequest
    {
        public string DockerRepositoryUrl { get; set; }
        public string DockerBranch { get; set; }
        public string BtcPayDockerImage { get; set; }
        public List<string> AdditionalFragments { get; set; }
        public List<string> Coins { get; set; }
        public string LetsEncryptEmail { get; set; }
        public string Lightning { get; set; }
        public string LightningAlias { get; set; }
        public string Network { get; set; }
        public string ReverseProxy { get; set; }
        public string BtcpayGeneratorDockerImage { get; set; }
        public string LibrePatronHost { get; set; }
        public string WoocommerceHost { get; set; }
        public string BtcTransmuterHost { get; set; }
        public string BtcpayHost { get; set; }
    }

    public class SetSSHDetailsRequest
    {
        public string Host { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class CheckHostRequest
    {
        public string VpsHost { get; set; }
        public string Check { get; set; }
    }
}