import {Module, Mutation, VuexModule} from "vuex-module-decorators";
import {HostsConfig, MainConfig, SetupConfig} from "@/installer/models/installer.models";
import {Utils} from "@/utils";

@Module({name: "config"})
export default class ConfigModule extends VuexModule implements SetupConfig, MainConfig, HostsConfig {
    public dockerRepositoryUrl: string = "https://github.com/btcpayserver/btcpayserver-docker";
    public dockerBranch: string = "master";
    public btcPayDockerImage: string = "";
    public additionalFragments: string[] = [];
    public coins: string[] = ["BTC"];
    public letsEncryptEmail: string = "";
    public lightning: "clightning" | "lnd" | "none" = "none";
    public lightningAlias: string = "";
    public network: "regtest" | "testnet" | "mainnet" = "mainnet";
    public reverseProxy: "nginx" | "traefik" = "nginx";
    public btcpayGeneratorDockerImage: string = "";
    public librePatronHost: string = "";
    public woocommerceHost: string = "";
    public btcpayHost: string = "";
    public btcTransmuterHost: string = "";

    public btcpayHostValid: boolean = false;
    public librePatronHostValid: boolean = false;
    public woocommerceHostValid: boolean = false;
    public btcTransmuterHostValid: boolean = false;

    @Mutation
    public setPreConfig(request: SetupConfig) {
        Utils.VueAssign(request, this);
    }

    @Mutation
    public setMainConfig(request: MainConfig) {
        Utils.VueAssign(request, this);
    }

    @Mutation
    public setAdditionalFragments(request: string[]) {
        this.additionalFragments = request;
    }

    @Mutation
    public setHosts(request: HostsConfig) {
        Utils.VueAssign(request, this);
    }
}
