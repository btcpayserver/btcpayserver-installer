import Vue from "vue";
import Vuex from "vuex";
import createPersistedState from "vuex-persistedstate";
import SSHModule from "@/installer/store-modules/ssh.module";
import ConfigModule from "@/installer/store-modules/config.module";
import AppModule from "@/store-modules/app.module";

Vue.use(Vuex);
const store  = new Vuex.Store({
    modules: {
        ssh: SSHModule,
        config: ConfigModule,
        app: AppModule
    },
    plugins: [
        createPersistedState({
            storage: window.sessionStorage,
        })
    ]
});
export default store;
