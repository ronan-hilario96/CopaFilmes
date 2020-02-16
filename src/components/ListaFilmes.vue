<template>
  <v-container>
    <v-row>
      <template v-for="filme in filmes">
        <v-col :key="filme.id" cols="3">
          <Filme :selecionado="filme.selecionado" :titulo="filme.titulo" :ano="filme.ano" />
        </v-col>
      </template>
    </v-row>
  </v-container>
</template>

<script>
import Filme from "./Filme";
export default {
  props: ["filmes"],
  components: {
    Filme
  }
};
</script>

<style>
</style>