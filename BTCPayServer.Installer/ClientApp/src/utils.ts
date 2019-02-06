import {Vue} from "vue-property-decorator";

export class Utils {
    public static VueAssign<Y>(src: Y, target: any) {
        for (const x in src) {
            if (x) {
                Vue.set(target, x, src[x]);
            }
        }
    }

    public static TextRequiredRule(value: string): true | string {
        return !!value || "Required";
    }

    public static HostnameNoProtocolRule(value: string): true | string {
        return (value && !value.startsWith("http")) || "Do not prefix the host with http(s)://";
    }

}
