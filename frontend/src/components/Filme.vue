<template>
  <v-card class="mx-auto d-flex myCard" @click="selectedCard">
    <v-checkbox v-model="selecionado" @click="selectedCard"/>
    <v-card outlined class="romover-default">
      <v-card-title>{{filmesDados.titulo}}</v-card-title>
      <v-card-subtitle>{{filmesDados.ano}}</v-card-subtitle>
    </v-card>
  </v-card>
</template>

<script>
import { mapState } from "vuex";
export default {
  data: () => ({
    selecionado: false,
    filmesDados: {}
  }),
  props: ["id", "titulo", "ano"],
  computed: {
    ...mapState(["filmes", "filmesSelecionados"])
  },
  methods: {
    adicionar: function() {
      let _id = this.id;
      if (this.filmesSelecionados.length < 8) 
      {
        if (!this.filmesSelecionados.filter(x => x.id == _id).length) 
        {
          let filmeSelecionado = this.filmes.filter(x => x.id == _id);

          this.$store.commit("adicionarFilme", filmeSelecionado[0]);

          this.selecionado = true;
        }
      }
    },
    remover: function() {
      let _id = this.id;
      let filmeSelecionado = this.filmesSelecionados.filter(x => x.id == _id)
      if (filmeSelecionado.length) 
      {
        this.$store.commit("removerFilme", filmeSelecionado[0]);

        this.selecionado = false;
      }
    },
    selectedCard: function() {
      !this.selecionado ? this.adicionar() : this.remover()
    }
  },
  beforeMount() {
    this.filmesDados.titulo = this.titulo;
    this.filmesDados.ano = this.ano;
    let _id = this.id;
    let selecionado = this.filmesSelecionados.filter(x => x.id == _id)

    this.selecionado = !!selecionado.length
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
  padding: 20px 15px;
  align-items: baseline;
}
</style>
