<template>
  <v-app>
    <v-content>
      <ListaFilmes :filmes="filmes" />
    </v-content>
  </v-app>
</template>

<script>
import ListaFilmes from "./components/ListaFilmes";
import axios from "axios";

export default {
  name: "App",
  data: () => ({
    filmes: []
  }),
  components: {
    ListaFilmes
  },
  methods: {
    ObterFilmes: async function () {
      // TODO: tratar erro
      let listaFilmes = await axios.get("https://localhost:44306/v1/CopaFilmes/Obter");

      if(listaFilmes.data.length)
      {
        for(let posicao = 0; posicao < listaFilmes.data.length; posicao++)
          listaFilmes.data[posicao].selecionado = false
        this.filmes = listaFilmes.data
      }
    }
  },
  mounted() {
    this.ObterFilmes();
  }
};
</script>
