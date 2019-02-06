<template>

    <v-container fluid>
        <v-layout column>
            <v-flex xs12>
                <v-btn @click="install" :disabled="loading">{{loading? "Deploying": "Deploy"}}</v-btn>

                <v-progress-circular
                        v-if="loading"
                        :size="70"
                        :width="7"
                        indeterminate
                ></v-progress-circular>
            </v-flex>

            <v-flex xs12>
                <h5>Output:</h5>
                <pre>
                        <template v-for="log of logs">{{log}}<br/></template>
                    </pre>
            </v-flex>
        </v-layout>
    </v-container>

</template>

<script lang="ts">
    import {Component, Vue} from "vue-property-decorator";
    import ConfigModule from "../store-modules/config.module";
    import {getModule} from "vuex-module-decorators";
    import installerHub from "@/installer/services/installer.hub";
    import SSHModule from "@/installer/store-modules/ssh.module";

    @Component
    export default class Deploy extends Vue {
        public loading: boolean = false;
        public logs: string[] = [];
        private configModule = getModule(ConfigModule, this.$store);
        private sshModule = getModule(SSHModule, this.$store);


        public install() {
            this.loading = true;
            installerHub.Install({
                additionalFragments: this.configModule.additionalFragments,
                btcPayDockerImage: this.configModule.btcPayDockerImage,
                btcpayGeneratorDockerImage: this.configModule.btcpayGeneratorDockerImage,
                btcpayHost: this.configModule.btcpayHost,
                coins: this.configModule.coins,
                dockerBranch: this.configModule.dockerBranch,
                dockerRepositoryUrl: this.configModule.dockerRepositoryUrl,
                letsEncryptEmail: this.configModule.letsEncryptEmail,
                librePatronHost: this.configModule.librePatronHost,
                lightning: this.configModule.lightning,
                lightningAlias: this.configModule.lightningAlias,
                network: this.configModule.network,
                reverseProxy: this.configModule.reverseProxy,
                woocommerceHost: this.configModule.woocommerceHost,
                host: this.sshModule.host,
                username: this.sshModule.username,
                password: this.sshModule.password
            })
                .subscribe({
                    next: (item: string) => {
                        this.logs.push(item);
                    },
                    complete: () => {
                        this.logs.push("Install Ended");

                        this.loading = false;
                    },
                    error: (err: any) => {
                        this.logs.push(`${err}`);
                    }
                });
        }
    }
</script>