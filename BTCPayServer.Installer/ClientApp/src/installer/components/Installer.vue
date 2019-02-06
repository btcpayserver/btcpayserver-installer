<template>
    <div>
        <v-stepper v-model="currentStep" >
            <v-stepper-header >
                <v-stepper-step :complete="currentStep > 1" step="1" @click="goToStep(1)">SSH Details</v-stepper-step>
                <v-divider></v-divider>
                <v-stepper-step :complete="currentStep > 2" step="2" @click="goToStep(2)">BTCPay Config</v-stepper-step>
                <v-divider></v-divider>
                <v-stepper-step :complete="currentStep > 3" step="3" @click="goToStep(3)">Additional Settings
                </v-stepper-step>
                <v-divider></v-divider>
                <v-stepper-step :complete="currentStep > 4" step="4" @click="goToStep(4)">Deploy</v-stepper-step>
            </v-stepper-header>

            <v-stepper-items>
                <v-stepper-content step="1">
                    <SSHDetails @submit="next"></SSHDetails>
                </v-stepper-content>
                <v-stepper-content step="2">
                    <BtcPayConfig @submit="next" @previous="previous"></BtcPayConfig>
                </v-stepper-content>
                <v-stepper-content step="3">
                    <AdditionalFragmentsSelector @submit="next" @previous="previous"></AdditionalFragmentsSelector>
                </v-stepper-content>
                <v-stepper-content step="4">
                    <Deploy @previous="previous"></Deploy>
                </v-stepper-content>
            </v-stepper-items>
        </v-stepper>
        <Console></Console>
    </div>
</template>

<script lang="ts">
    import {Component, Vue} from "vue-property-decorator";
    import SSHDetails from "@/installer/components/SSHDetails.vue";
    import BtcPayConfig from "@/installer/components/BtcPayConfig.vue";
    import {getModule} from "vuex-module-decorators";
    import AppModule from "@/store-modules/app.module";
    import AdditionalFragmentsSelector from "@/installer/components/AdditionalFragmentsSelector.vue";
    import Console from "@/installer/components/Console.vue";
    import Deploy from "@/installer/components/Deploy.vue";

    @Component({
        components: {
            Console,
            SSHDetails,
            AdditionalFragmentsSelector,
            BtcPayConfig,
            Deploy
        }
    })
    export default class Installer extends Vue {
        private appModule = getModule(AppModule, this.$store);

        public get currentStep() {
            return this.appModule.currentStep;
        }

        public set currentStep(val: number) {
            this.appModule.setStep(val);
        }

        public next() {
            this.currentStep = this.currentStep + 1;
        }

        public previous() {
            this.currentStep = this.currentStep - 1;
        }

        public goToStep(step: number) {
            if (this.currentStep > step) {
                this.currentStep = step;
            }
        }
    }
</script>
