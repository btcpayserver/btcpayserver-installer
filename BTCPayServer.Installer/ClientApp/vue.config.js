var tsNameof = require("ts-nameof");

module.exports = {
    transpileDependencies: [
        'vuex-module-decorators'
    ],
    chainWebpack: config => {
        config.module.rule("ts").use("ts-loader").tap(options => {
            options.getCustomTransformers = () => ({ before: [tsNameof] });
            return options;
        });
    }
};