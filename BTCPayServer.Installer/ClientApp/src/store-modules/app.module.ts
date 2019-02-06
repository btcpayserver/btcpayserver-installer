import {getModule, Module, Mutation, MutationAction, VuexModule} from "vuex-module-decorators";
import store from "@/store";

@Module({name: "app"})
export default class AppModule extends VuexModule {
    public dark: boolean = true;
    public currentStep: number = 1;

    @Mutation
    public setDark(dark: boolean) {
        this.dark = dark;
    }

    @Mutation
    public setStep(step: number) {
        this.currentStep = step;
    }
}
