<template>
  <div class="home">
    <Installer></Installer>
  </div>
</template>

<script lang="ts">
import { Component, Vue } from "vue-property-decorator";
import Installer from "@/installer/components/Installer.vue";

@Component({
  components: {
     Installer
  },
})
export default class Home extends Vue {}
</script>
