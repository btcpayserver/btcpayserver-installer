<template>
    <v-form v-model="valid" ref="form">
        <v-text-field
                v-model="dockerRepositoryUrl"
                label="Docker Repo Url"
                :rules="rules"
                required
        ></v-text-field>

        <v-text-field
                v-model="dockerBranch"
                label="Docker Repo Branch"
                :rules="rules"
                required
        ></v-text-field>

        <v-text-field
                v-model="btcPayDockerImage"

                label="BTCPay Docker Custom Image"
        ></v-text-field>
        <v-text-field
                v-model="btcpayGeneratorDockerImage"
                :disabled="buildLocally"
                label="BTCPay Docker Generator Custom Image"
        ></v-text-field>
        <v-checkbox v-model="buildLocally" label="Build BTCPay Docker Generator locally"></v-checkbox>

    </v-form>
</template>

<script lang="ts">
    import {Component, Emit, Vue} from "vue-property-decorator";
    import {VForm} from "vuetify/lib";
    import {SetupConfig} from "@/installer/models/installer.models";
    import ConfigModule from "@/installer/store-modules/config.module";
    import {getModule} from "vuex-module-decorators";
    import {Utils} from "@/utils";

    @Component({})
    export default class PreSetupConfig extends Vue implements SetupConfig {
        public $refs!: Vue["$refs"] & {
            form: VForm
        };

        public valid: boolean = false;

        public get rules() {
            return [Utils.TextRequiredRule];
        }

        public get buildLocally() {
            return this.btcpayGeneratorDockerImage === "btcpayserver/docker-compose-generator:local";
        }

        public set buildLocally(val: boolean) {
            if (val) {
                this.btcpayGeneratorDockerImage = "btcpayserver/docker-compose-generator:local";
            } else {
                this.btcpayGeneratorDockerImage = "";
            }
        }

        public get dockerBranch(): string {
            return this.configModule.dockerBranch;
        }

        public set dockerBranch(value: string) {
            this.configModule.setPreConfig({
                dockerRepositoryUrl: this.dockerRepositoryUrl,
                dockerBranch: value,
                btcPayDockerImage: this.btcPayDockerImage,
                btcpayGeneratorDockerImage: this.btcpayGeneratorDockerImage
            });
        }

        public get dockerRepositoryUrl(): string {
            return this.configModule.dockerRepositoryUrl;
        }

        public set dockerRepositoryUrl(value: string) {
            this.configModule.setPreConfig({
                dockerRepositoryUrl: value,
                dockerBranch: this.dockerBranch,
                btcPayDockerImage: this.btcPayDockerImage,
                btcpayGeneratorDockerImage: this.btcpayGeneratorDockerImage
            });
        }

        public get btcPayDockerImage(): string {
            return this.configModule.btcPayDockerImage;
        }

        public set btcPayDockerImage(value: string) {
            this.configModule.setPreConfig({
                dockerRepositoryUrl: this.dockerRepositoryUrl,
                dockerBranch: this.dockerBranch,
                btcPayDockerImage: value,
                btcpayGeneratorDockerImage: this.btcpayGeneratorDockerImage
            });
        }

        public get btcpayGeneratorDockerImage(): string | undefined {
            return this.configModule.btcpayGeneratorDockerImage;
        }

        public set btcpayGeneratorDockerImage(value: string | undefined) {
            this.configModule.setPreConfig({
                dockerRepositoryUrl: this.dockerRepositoryUrl,
                dockerBranch: this.dockerBranch,
                btcPayDockerImage: this.btcPayDockerImage,
                btcpayGeneratorDockerImage: value
            });
        }

        private configModule = getModule(ConfigModule, this.$store);
    }
</script>
