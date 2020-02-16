<template>
  <v-card class="mx-auto d-flex myCard" @click="selectedCard">
    <v-checkbox v-model="filmesDados.selecionado" />
    <v-card outlined class="romover-default">
      <v-card-title class="headline">{{filmesDados.titulo}}</v-card-title>
      <v-card-subtitle class="pb-0">{{filmesDados.ano}}</v-card-subtitle>
    </v-card>
  </v-card>
</template>

<script>
export default {
  data: () => ({
    filmesDados: {
      selecionado: false,
      titulo: "Titulo",
      ano: 2018
    }
  }),
  props: ["selecionado", "titulo", "ano"],
  methods: {
    selectedCard: function() {
      this.filmesDados.selecionado = !this.filmesDados.selecionado;
    }
  },
  mounted() {
    this.filmesDados.selecionado = this.selecionado;
    this.filmesDados.titulo = this.titulo;
    this.filmesDados.ano = this.ano;
  }
};
</script>

<style scoped>
.romover-default {
  border: none !important;
  background: transparent !important;
}
.myCard {
    height: 100%;
}
</style>
