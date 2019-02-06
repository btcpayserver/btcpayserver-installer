<template>
    <v-form v-model="valid" ref="form" lazy-validation>
        <HostnameEntry field-label="BTCPay Host"
                       v-model="btcpayHost"
                       :host-valid="btcpayHostValid"
                       @host-valid="btcpayHostValid = $event"></HostnameEntry>
        <v-select
                :items="availableCoins"
                v-model="coins"
                item-text="text"
                item-value="value"
                multiple
                label="Cryptos"
        ></v-select>


        <v-select
                :items="networks"
                v-model="network"
                item-text="text"
                item-value="value"
                label="Network"
        ></v-select>

        <v-select
                :items="reverseProxies"
                v-model="reverseProxy"
                item-text="text"
                item-value="value"
                label="Reverse Proxy"
        ></v-select>
        <v-text-field
                v-if="reverseProxy && reverseProxy !=='none'"
                v-model="letsEncryptEmail"
                type="email"
                label="Let's Encrypt Email Notifier"
        ></v-text-field>

        <v-select
                :items="lightningImplementations"
                v-model="lightning"
                item-text="text"
                item-value="value"
                label="Lightning"
        ></v-select>

        <v-text-field
                v-if="lightning && lightning !=='none'"
                v-model="lightningAlias"
                label="Lightning Node Alias"
        ></v-text-field>


        <v-expansion-panel>
            <v-expansion-panel-content>
                <div slot="header">Advanced</div>

                <PreSetupConfig ref="advancedForm"></PreSetupConfig>
            </v-expansion-panel-content>
        </v-expansion-panel>

        <v-btn :disabled="!valid" @click="submit">
            Continue
        </v-btn>
        <v-btn @click="previous">
            Previous
        </v-btn>

    </v-form>
</template>

<script lang="ts">
    import {Component, Emit, Vue} from "vue-property-decorator";
    import {VForm} from "vuetify/lib";
    import {MainConfig} from "@/installer/models/installer.models";
    import ConfigModule from "@/installer/store-modules/config.module";
    import {getModule} from "vuex-module-decorators";
    import HostnameEntry from "@/installer/components/HostnameEntry.vue";
    import PreSetupConfig from "@/installer/components/PreSetupConfig.vue";
    import {Utils} from "@/utils";

    @Component({
        components: {
            HostnameEntry,
            PreSetupConfig
        }
    })
    export default class BtcPayConfig extends Vue implements MainConfig {
        public $refs!: Vue["$refs"] & {
            form: VForm
        };

        public valid: boolean = false;

        public get additionalFragments() {
            return this.configModule.additionalFragments;
        }

        public get availableCoins() {
            return [
                {text: "BTC", value: "BTC"},
                {text: "LTC", value: "LTC"}
            ];
        }

        public get networks() {
            return [
                {text: "Mainnet", value: "mainnet"},
                {text: "Testnet", value: "testnet"},
                {text: "Regtest", value: "regtest"}
            ];

        }

        public get reverseProxies() {
            return [
                {text: "Nginx (Recommended)", value: "nginx"},
                {text: "Traefik (limited support)", value: "traefik"}
            ];
        }

        public get lightningImplementations() {
            return [
                {text: "C-Lightning", value: "clightning"},
                {text: "LND", value: "lnd"},
                {text: "No Lightning", value: "none"}
            ];
        }

        public get coins() {
            return this.configModule.coins;
        }

        public set coins(val: string[]) {
            this.configModule.setMainConfig({
                additionalFragments: this.additionalFragments,
                coins: val,
                letsEncryptEmail: this.letsEncryptEmail,
                lightning: this.lightning,
                lightningAlias: this.lightningAlias,
                network: this.network,
                reverseProxy: this.reverseProxy
            });
        }

        public get letsEncryptEmail() {
            return this.configModule.letsEncryptEmail;
        }

        public set letsEncryptEmail(value: string) {
            this.configModule.setMainConfig({
                additionalFragments: this.additionalFragments,
                coins: this.coins,
                letsEncryptEmail: value,
                lightning: this.lightning,
                lightningAlias: this.lightningAlias,
                network: this.network,
                reverseProxy: this.reverseProxy
            });
        }

        public get lightning() {
            return this.configModule.lightning;
        }

        public set lightning(value) {
            this.configModule.setMainConfig({
                additionalFragments: this.additionalFragments,
                coins: this.coins,
                letsEncryptEmail: this.letsEncryptEmail,
                lightning: value,
                lightningAlias: this.lightningAlias,
                network: this.network,
                reverseProxy: this.reverseProxy
            });
        }

        public get lightningAlias() {
            return this.configModule.lightningAlias;
        }

        public set lightningAlias(value) {
            this.configModule.setMainConfig({
                additionalFragments: this.additionalFragments,
                coins: this.coins,
                letsEncryptEmail: this.letsEncryptEmail,
                lightning: this.lightning,
                lightningAlias: value,
                network: this.network,
                reverseProxy: this.reverseProxy
            });
        }

        public get network() {
            return this.configModule.network;
        }

        public set network(value) {
            this.configModule.setMainConfig({
                additionalFragments: this.additionalFragments,
                coins: this.coins,
                letsEncryptEmail: this.letsEncryptEmail,
                lightning: this.lightning,
                lightningAlias: this.lightningAlias,
                network: value,
                reverseProxy: this.reverseProxy
            });
        }

        public get reverseProxy() {
            return this.configModule.reverseProxy;
        }

        public set reverseProxy(value) {
            this.configModule.setMainConfig({
                additionalFragments: this.additionalFragments,
                coins: this.coins,
                letsEncryptEmail: this.letsEncryptEmail,
                lightning: this.lightning,
                lightningAlias: this.lightningAlias,
                network: this.network,
                reverseProxy: value
            });
        }

        public get btcpayHost() {
            return this.configModule.btcpayHost;
        }

        public set btcpayHost(value: string) {
            debugger;
            this.configModule.setHosts({
                btcpayHost: value,
                btcpayHostValid: this.configModule.btcpayHostValid,
                librePatronHost: this.configModule.librePatronHost,
                librePatronHostValid: this.configModule.librePatronHostValid,
                woocommerceHost: this.configModule.woocommerceHost,
                woocommerceHostValid: this.configModule.woocommerceHostValid
            });
        }

        public get btcpayHostValid() {
            return this.configModule.btcpayHostValid;
        }

        public set btcpayHostValid(value: boolean) {
            this.configModule.setHosts({
                btcpayHost: this.configModule.btcpayHost,
                btcpayHostValid: value,
                librePatronHost: this.configModule.librePatronHost,
                librePatronHostValid: this.configModule.librePatronHostValid,
                woocommerceHost: this.configModule.woocommerceHost,
                woocommerceHostValid: this.configModule.woocommerceHostValid
            });
        }

        public get rules() {
            return [Utils.TextRequiredRule];
        }

        private configModule = getModule(ConfigModule, this.$store);

        @Emit()
        public submit() {
        }

        @Emit()
        public previous() {
        }
    }
</script>
