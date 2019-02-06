<template>
    <v-form v-model="valid" ref="form">
        <v-text-field
                v-model="host"
                label="VPS Host"
                :rules="rules"
                required
        ></v-text-field>
        <v-text-field
                v-model="username"
                label="SSH User"
                :rules="rules"
                required
        ></v-text-field>

        <v-text-field
                v-model="password"
                label="SSH Password"
                :rules="rules"
                :append-icon="showPassword ? 'fa-eye' : 'fa-eye-slash'"
                :type="showPassword ? 'text' : 'password'"
                @click:append="showPassword = !showPassword"
                required
        ></v-text-field>

        <v-btn :disabled="!sshValid || loading" @click="submit">
            Continue
        </v-btn>
        <v-btn :disabled="!valid || loading" @click="test" :loading="loading">
            Test
        </v-btn>
        <v-btn @click="clear">
            Clear
        </v-btn>

        <v-snackbar
                v-model="showNotificationMessage"
                :timeout="5000"
                right top
        >
            {{ notificationMessage }}
            <v-btn flat @click="showNotificationMessage = false">Close</v-btn>
        </v-snackbar>

    </v-form>
</template>

<script lang="ts">
    import {Component, Emit, Vue, Watch} from "vue-property-decorator";
    import {VForm} from "vuetify/lib";
    import {getModule} from "vuex-module-decorators";
    import SShModule from "@/installer/store-modules/ssh.module";
    import {Utils} from "@/utils";

    @Component({})
    export default class SSHDetails extends Vue {

        public showPassword: boolean = false;

        public $refs!: Vue["$refs"] & {
            form: VForm
        };
        public showNotificationMessage: boolean = false;
        public notificationMessage: string = "";

        public loading: boolean = false;
        public valid: boolean = false;

        public get sshValid() {
            return this.sshModule.sshValid;
        }

        public get rules() {
            return [Utils.TextRequiredRule];
        }

        public get username(): string {
            return this.sshModule.username;
        }

        public set username(value: string) {
            this.sshModule.setSSHDetails({
                username: value,
                password: this.password,
                host: this.host
            });
        }

        public get password(): string {
            return this.sshModule.password;
        }

        public set password(value: string) {
            this.sshModule.setSSHDetails({
                username: this.username,
                password: value,
                host: this.host
            });
        }

        public get host(): string {
            return this.sshModule.host;
        }

        public set host(value: string) {
            this.sshModule.setSSHDetails({
                username: this.username,
                password: this.password,
                host: value
            });
        }

        private sshModule = getModule(SShModule, this.$store);

        @Emit()
        public submit() {
        }

        @Watch("notificationMessage")
        public onNotificationMessage(val: string, oldVal: string) {

            this.showNotificationMessage = true;
        }

        public async test() {
            if (this.valid) {
                this.loading = true;
                await this.sshModule.testSSHDetails();
                if (this.sshValid) {
                    this.notificationMessage = "SSH Details valid";
                } else {
                    this.notificationMessage = "SSH Details suck";
                }
                this.loading = false;
            }
        }

        public clear() {
            this.$refs.form.reset();
        }
    }
</script>
