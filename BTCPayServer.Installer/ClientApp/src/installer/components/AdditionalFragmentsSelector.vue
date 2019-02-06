<template>
    <v-form v-model="valid" ref="form">
        <v-select
                :items="pruningOptions"
                v-model="pruningOption"
                item-text="text"
                item-value="value"
                label="Node pruning Options"
        ></v-select>

        <v-checkbox v-model="lndAutoPilot" label="Enable LND Autopilot" v-if="usingLND"></v-checkbox>
        <v-checkbox v-model="saveMemory" label="Enable Memory Optimizations"></v-checkbox>
        <v-checkbox v-model="librePatron" label="Enable LibrePatron"></v-checkbox>
        <HostnameEntry
                v-if="librePatron"
                v-model="librePatronHost"
                field-label="Libre Patron Hostname"
                :host-valid="librePatronHostValid"
                @host-valid="librePatronHostValid = $event"></HostnameEntry>
        <v-checkbox v-model="woocommerce" label="Enable WooCommerce"></v-checkbox>
        <HostnameEntry
                v-if="woocommerce"
                v-model="woocommerceHost"
                field-label="WooCommerce Hostname"
                :host-valid="woocommerceHostValid"
                @host-valid="woocommerceHostValid = $event"></HostnameEntry>
        <v-checkbox v-model="btcqbo" label="Enable Quick Books Integration"></v-checkbox>

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
    import ConfigModule from "@/installer/store-modules/config.module";
    import {getModule} from "vuex-module-decorators";
    import HostnameEntry from "@/installer/components/HostnameEntry.vue";
    import {Utils} from "@/utils";
    import installerHub from "@/installer/services/installer.hub";
    import SSHModule from "@/installer/store-modules/ssh.module";

    @Component({
        components: {
            HostnameEntry
        }
    })
    export default class AdditionalFragmentsSelector extends Vue {

        public valid: boolean = false;

        public get rules() {
            return [Utils.TextRequiredRule];
        }

        public get usingLND() {
            return this.configModule.lightning === "lnd";
        }

        public get additionalFragments() {
            return this.configModule.additionalFragments;
        }

        public get pruningOptions() {
            return [
                {text: "No Pruning", value: ""},
                {text: "Prune ", value: "opt-save-storage"},
                {text: "Prune Small", value: "opt-save-storage-s"},
                {text: "Prune Extra Small", value: "opt-save-storage-xs"},
                {text: "Prune Extra Extra Small (Lightning Not Supported)", value: "opt-save-storage-xxs"}
            ];
        }

        public get pruningOption() {
            for (const fragment of this.additionalFragments) {
                for (const option of this.pruningOptions) {
                    if (fragment === option.value) {
                        return fragment;
                    }
                }
            }
            return "";
        }

        public set pruningOption(val: string) {
            this.configModule.setAdditionalFragments(
                this.constructAdditionalFragments(
                    val,
                    this.lndAutoPilot, this.saveMemory, this.librePatron, this.woocommerce, this.btcqbo));
        }


        public get lndAutoPilot() {
            for (const fragment of this.additionalFragments) {
                if (fragment === "opt-lnd-autopilot") {
                    return true;
                }
            }
            return false;
        }

        public set lndAutoPilot(val: boolean) {
            this.configModule.setAdditionalFragments(
                this.constructAdditionalFragments(
                    this.pruningOption,
                    val,
                    this.saveMemory, this.librePatron, this.woocommerce, this.btcqbo));
        }

        public get saveMemory() {
            for (const fragment of this.additionalFragments) {
                if (fragment === "opt-save-memory ") {
                    return true;
                }
            }
            return false;
        }

        public set saveMemory(val: boolean) {
            this.configModule.setAdditionalFragments(
                this.constructAdditionalFragments(
                    this.pruningOption,
                    this.lndAutoPilot,
                    val, this.librePatron, this.woocommerce, this.btcqbo));
        }

        public get librePatron() {
            for (const fragment of this.additionalFragments) {
                if (fragment === "opt-add-librepatron") {
                    return true;
                }
            }
            return false;
        }

        public set librePatron(val: boolean) {
            this.configModule.setAdditionalFragments(
                this.constructAdditionalFragments(
                    this.pruningOption,
                    this.lndAutoPilot,
                    this.saveMemory,
                    val, this.woocommerce, this.btcqbo));
            if (!val) {
                this.librePatronHost = "";
            }

        }

        public get woocommerce() {
            for (const fragment of this.additionalFragments) {
                if (fragment === "opt-add-woocommerce") {
                    return true;
                }
            }
            return false;
        }

        public set woocommerce(val: boolean) {
            this.configModule.setAdditionalFragments(
                this.constructAdditionalFragments(
                    this.pruningOption,
                    this.lndAutoPilot,
                    this.saveMemory,
                    this.librePatron,
                    val, this.btcqbo));

            if (!val) {
                this.woocommerceHost = "";
            }
        }

        public get btcqbo() {
            for (const fragment of this.additionalFragments) {
                if (fragment === "opt-add-btcqbo") {
                    return true;
                }
            }
            return false;
        }

        public set btcqbo(val: boolean) {
            this.configModule.setAdditionalFragments(
                this.constructAdditionalFragments(
                    this.pruningOption,
                    this.lndAutoPilot,
                    this.saveMemory,
                    this.librePatron,
                    this.woocommerce,
                    val));
        }

        public get librePatronHost() {
            return this.configModule.librePatronHost;
        }

        public set librePatronHost(value: string) {
            this.configModule.setHosts({
                btcpayHost: value,
                btcpayHostValid: this.configModule.btcpayHostValid,
                librePatronHost: value,
                librePatronHostValid: this.configModule.librePatronHostValid,
                woocommerceHost: this.configModule.woocommerceHost,
                woocommerceHostValid: this.configModule.woocommerceHostValid
            });
        }

        public get librePatronHostValid() {
            return this.configModule.librePatronHostValid;
        }

        public set librePatronHostValid(value: boolean) {
            this.configModule.setHosts({
                btcpayHost: this.configModule.btcpayHost,
                btcpayHostValid: this.configModule.btcpayHostValid,
                librePatronHost: this.configModule.librePatronHost,
                librePatronHostValid: value,
                woocommerceHost: this.configModule.woocommerceHost,
                woocommerceHostValid: this.configModule.woocommerceHostValid
            });
        }

        public get woocommerceHost() {
            return this.configModule.woocommerceHost;
        }

        public set woocommerceHost(value: string) {
            this.configModule.setHosts({
                btcpayHost: value,
                btcpayHostValid: this.configModule.btcpayHostValid,
                librePatronHost: this.configModule.librePatronHost,
                librePatronHostValid: this.configModule.librePatronHostValid,
                woocommerceHost: value,
                woocommerceHostValid: this.configModule.woocommerceHostValid
            });
        }

        public get woocommerceHostValid() {
            return this.configModule.woocommerceHostValid;
        }

        public set woocommerceHostValid(value: boolean) {
            this.configModule.setHosts({
                btcpayHost: this.configModule.btcpayHost,
                btcpayHostValid: this.configModule.btcpayHostValid,
                librePatronHost: this.configModule.librePatronHost,
                librePatronHostValid: this.configModule.librePatronHostValid,
                woocommerceHost: this.configModule.woocommerceHost,
                woocommerceHostValid: value
            });
        }

        private configModule = getModule(ConfigModule, this.$store);

        @Emit()
        public submit() {
        }

        @Emit()
        public previous() {
        }

        private constructAdditionalFragments(pruningOption: string,
                                             lndAutoPilot: boolean,
                                             saveMemory: boolean,
                                             librePatron: boolean,
                                             woocommerce: boolean,
                                             btcqbo: boolean) {
            const result = [];

            if (pruningOption) {
                result.push(pruningOption);
            }
            if (lndAutoPilot) {
                result.push("opt-lnd-autopilot");
            }
            if (saveMemory) {
                result.push("opt-save-memory");
            }
            if (librePatron) {
                result.push("opt-add-librepatron");
            }
            if (btcqbo) {
                result.push("opt-add-btcqbo");
            }
            if (woocommerce) {
                result.push("opt-add-woocommerce");
            }

            return result;
        }

    }
</script>