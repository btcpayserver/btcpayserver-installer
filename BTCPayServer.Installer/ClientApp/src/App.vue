<template>
    <v-app :dark="isDark">
        <v-toolbar app>
            <v-toolbar-title class="headline text-uppercase">
                <span>BTCPayServer</span>
                <span class="font-weight-light">Installer</span>
            </v-toolbar-title>
            <v-spacer></v-spacer>
            <v-btn
                    flat
                    href="https://btcpayserver.org"
                    target="_blank"
            >
                Official Website
            </v-btn>

            <v-btn
                    flat
                    href="https://chat.btcpayserver.org"
                    target="_blank"
            >
                Chat
            </v-btn>
            <v-btn
                    flat
                    @click="toggleDark"
            >
                {{isDark? "Light" : "Dark"}}
            </v-btn>

        </v-toolbar>

        <v-content>
            <v-container fluid>
                <router-view></router-view>
            </v-container>
        </v-content>
    </v-app>
</template>

<script lang="ts">
    import {Component, Vue} from "vue-property-decorator";
    import AppModule from "@/store-modules/app.module";
    import {getModule} from "vuex-module-decorators";

    @Component({})
    export default class App extends Vue {

        private appModule = getModule(AppModule, this.$store);
        public get isDark(): boolean {
            return this.appModule.dark;
        }

        public toggleDark() {
            this.appModule.setDark(!this.isDark);
        }
    }
</script>
