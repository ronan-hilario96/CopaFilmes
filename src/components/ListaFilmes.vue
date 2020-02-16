<template>
  <v-container>
    <v-row>
      <template v-for="filme in filmes">
        <v-col :key="filme.id" cols="3">
          <Filme :id="filme.id" :titulo="filme.titulo" :ano="filmes.ano" />
        </v-col>
      </template>
    </v-row>
  </v-container>
</template>

<script>
import Filme from "./Filme";
import { mapState } from "vuex";

export default {
  components: {
    Filme
  },
  data: () => ({}),
  computed: {
    ...mapState(['filmes', 'filmesSelecionados'])
  },
  mounted() {
    this.$store.dispatch("obterFilmes")
  }
};
</script>

<style>
</style>