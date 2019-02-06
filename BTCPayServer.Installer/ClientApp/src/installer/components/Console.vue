<template>

    <v-card class="card--flex-toolbar" v-if="canUseConsole">
        <v-toolbar card prominent>
            <v-toolbar-title class="body-2 grey--text">Console</v-toolbar-title>

            <v-spacer></v-spacer>

            <v-btn @click="checkSpace" :disabled="loading">
                Check Space
            </v-btn>
            <v-btn @click="checkDocker" :disabled="loading">
                Check Docker
            </v-btn>

        </v-toolbar>

        <v-divider></v-divider>

        <v-container align-content-center align-center>
            <v-layout align-content-center align-center>
                <v-flex align-self-center>
                    <v-progress-circular
                            class="mx-auto"
                            v-if="loading"
                            :size="70"
                            :width="7"
                            indeterminate
                    ></v-progress-circular>
                    <pre v-if="!loading" class="mx-auto">{{data}}</pre>
                </v-flex>
            </v-layout>
        </v-container>

    </v-card>

</template>

<script lang="ts">
    import {Component, Vue} from "vue-property-decorator";
    import SSHModule from "@/installer/store-modules/ssh.module";
    import {getModule} from "vuex-module-decorators";
    import installerHub from "@/installer/services/installer.hub";

    @Component
    export default class Console extends Vue {
        public data: string = "";
        public loading = false;

        public get canUseConsole() {
            return this.sshModule.sshValid;
        }

        private sshModule = getModule(SSHModule, this.$store);

        public async checkSpace() {
            this.loading = true;
            try {
                this.data = await installerHub.checkSpace({
                    host: this.sshModule.host,
                    username: this.sshModule.username,
                    password: this.sshModule.password
                });
            } catch {
                this.data = "An Error Occurred.";
            } finally {
                this.loading = false;
            }
        }

        public async checkDocker() {
            this.loading = true;
            try {
                this.data = await installerHub.checkDocker({
                    host: this.sshModule.host,
                    username: this.sshModule.username,
                    password: this.sshModule.password
                });
            } catch {
                this.data = "An Error Occurred.";
            } finally {
                this.loading = false;
            }
        }
    }
</script>