<template>
  <v-card class="text-center cartao">
    <v-card-subtitle class="subtitulo">{{ subtitulo}}</v-card-subtitle>
    <v-card-title><p class="titulo">{{ titulo }}</p></v-card-title>
    <v-card-text class="text">
       <slot></slot>
    </v-card-text>
  </v-card>
</template>

<script>
import { mapState } from "vuex";
export default {
  data: () => ({
  }),
  props: ["subtitulo", "titulo"],
  computed: {
    ...mapState(["filmes", "filmesSelecionados"])
  },
  methods: {
  },
  mounted() {
  }
};
</script>

<style scoped>
.titulo {
  margin: 60px auto;
  font-size: 48px;
}
.subtitulo {
  font-size: 38px;
}
.text {
  font-size: 28px;
  max-width: 1050px;
  margin: 0 auto;
  line-height: 1.5;
}
</style>
