<template>
 <div>
     <v-text-field
             v-model="internalValue"
             :label="fieldLabel"
             :validate-on-blur="true"
             :append-outer-icon="hostValid ?'fa-check' : 'fa-redo'"
             @click:append-outer="checkHost"
             :rules="rules"
     ></v-text-field>
 </div>
</template>

<script lang="ts">
    import {Component, Prop, Vue, Watch} from "vue-property-decorator";
    import installerHub from "@/installer/services/installer.hub";
    import {Utils} from "@/utils";
    import SSHModule from "@/installer/store-modules/ssh.module";
    import {getModule} from "vuex-module-decorators";

    @Component
    export default class HostnameEntry extends Vue {
        @Prop(String)
        public fieldLabel!: string;

        @Prop(String)
        public value!: string;

        @Prop(Boolean)
        public hostValid!: boolean;

        public get internalValue() {
            return this.value;
        }

        public set internalValue(value: string) {
            this.$emit("input", value);
        }

        public get rules() {

            return [
                Utils.TextRequiredRule,
                Utils.HostnameNoProtocolRule,
                this.hostnameValidHost.bind(this)];
        }

        private sshModule = getModule(SSHModule, this.$store);

        @Watch(nameof<HostnameEntry>((x: HostnameEntry) => x.value))
        public async onHostChange() {
            await this.checkHost();
        }

        public async checkHost() {
            const result = await installerHub.checkHost({check: this.value, vpsHost: this.sshModule.host});
            this.$emit("host-valid", result);
        }

        private hostnameValidHost(value: string) {
            return this.hostValid || "Hostname does not match the IP of your VPS or is not a proper host";
        }

    }
</script>