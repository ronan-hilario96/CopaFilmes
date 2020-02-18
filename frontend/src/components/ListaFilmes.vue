<template>
  <v-row>
    <template v-for="filme in filmes">
      <v-col :key="filme.id" cols="12" xs="12" sm="6" md="3" lg="3" xl="3">
        <Filme :id="filme.id" :titulo="filme.titulo" :ano="filme.ano" />
      </v-col>
    </template>
  </v-row>
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