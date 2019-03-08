export interface ISshBasicDetails {
    host: string;
    username: string;
    password: string;
}

export interface SetupConfig {
    dockerRepositoryUrl: string;
    dockerBranch: string;
    btcpayGeneratorDockerImage?: string;
    btcPayDockerImage?: string;
}

export interface MainConfig {
    coins: string[];
    network: "regtest" | "testnet" | "mainnet";
    lightning?: "clightning" | "lnd" | "none";
    lightningAlias?: string;
    letsEncryptEmail?: string;
    reverseProxy: "nginx" | "traefik";
    additionalFragments: string[];
}

export interface ICheckHostRequest {
    vpsHost: string;
    check: string;
}

export interface HostsConfig {
    librePatronHost: string;
    librePatronHostValid: boolean;
    woocommerceHost: string;
    woocommerceHostValid: boolean;
    btcpayHost: string;
    btcpayHostValid: boolean;
    btcTransmuterHost: string;
    btcTransmuterHostValid: boolean;
}


export interface InstallRequest extends ISshBasicDetails {
    dockerRepositoryUrl: string;
    dockerBranch: string;
    btcPayDockerImage: string;
    additionalFragments: string[];
    coins: string[];
    letsEncryptEmail: string;
    lightning: string;
    lightningAlias: string;
    network: string;
    reverseProxy: string;
    btcpayGeneratorDockerImage: string;
    librePatronHost: string;
    woocommerceHost: string;
    btcpayHost: string;
}
