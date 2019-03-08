import * as signalR from "@aspnet/signalr";
import {HubConnection} from "@aspnet/signalr";
import {ICheckHostRequest, InstallRequest, ISshBasicDetails} from "@/installer/models/installer.models";


export class InstallerHub {
    private connection: HubConnection;

    constructor() {
        this.connection = new signalR.HubConnectionBuilder()
            .withUrl("/api/v1/installer/hub")
            .build();

        this.connection.onclose(() => {
            console.error("Connection was closed. Attempting reconnect in 2s");
            setTimeout(this.connectToHub.bind(this), 2000);
        });
        this.connectToHub();
    }

    public async testSSHDetails(request: ISshBasicDetails) {
        try {
            return await this.connection.invoke<boolean>("TestSshDetails", request);
        } catch {
            return false;
        }
    }

    public Install(installConfig: InstallRequest) {
        return this.connection.stream<string>("Install", installConfig);
    }

    public async checkHost(request: ICheckHostRequest): Promise<boolean> {
        try {
            const result = await this.connection.invoke<boolean>("CheckHost", request);
            console.warn("CHECK HOST", request, result);
            return result;
        } catch {
            return false;
        }
    }

    public async checkSpace(request: ISshBasicDetails) {
        return await this.connection.invoke<string>("CheckSpace", request);
    }

    public async checkDocker(request: ISshBasicDetails) {
        return await this.connection.invoke<string>("CheckDocker", request);
    }

    private connectToHub() {
        this.connection
            .start()
            .catch((err: string) => {
                console.error("Could not connect to backend. Retrying in 2s", err);
                setTimeout(this.connectToHub.bind(this), 2000);
            });
        this.connection.start().catch((err) => console.warn);
    }
}

const installerHub = new InstallerHub();
export default installerHub;
