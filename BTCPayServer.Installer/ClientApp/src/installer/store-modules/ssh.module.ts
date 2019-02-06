import { Action, Module, Mutation, VuexModule } from "vuex-module-decorators";
import { ISshBasicDetails } from "@/installer/models/installer.models";
import installerHub from "@/installer/services/installer.hub";

@Module({ name: "ssh" })
export default class SSHModule extends VuexModule implements ISshBasicDetails {
    public host: string = "";
    public password: string = "";
    public username: string = "root";
    public sshValid: boolean = false;

    @Mutation
    public setSSHDetails(request: ISshBasicDetails) {
        this.username = request.username;
        this.password = request.password;
        this.host = request.host;
        this.sshValid = false;
    }

    @Mutation
    public setSshValid(valid: boolean) {
        this.sshValid = valid;
    }

    @Action({ commit: "setSshValid" })
    public async testSSHDetails() {
        return await installerHub.testSSHDetails({
            host: this.host,
            password: this.password,
            username: this.username
        });
    }
}