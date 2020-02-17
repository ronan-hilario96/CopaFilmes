<template>
  <v-container>
    <CardPrincipal titulo="CAMPEONATO DE FILMES" subtitulo="Resultado Final">
      <p>Veja o resultado final do Campeonato de filmes de forma simples e rápida</p>
    </CardPrincipal>
    <CardCampeao v-for="(campeao, index) in campeoes" :key="campeao.id" :titulo="campeao.titulo" :posicao="index+1+'°'"/>
  </v-container>
</template>

<script>
import CardPrincipal from "../components/CardPrincipal";
import CardCampeao from "../components/CardCampeao";
import { mapState } from "vuex";
import axios from 'axios'

export default {
  name: 'Resultado',
  components: {
    CardPrincipal,
    CardCampeao
  },
  data: () => ({
    campeoes: []
  }),
  computed: {
    ...mapState(["urlCopaFilmles", "filmesSelecionados"]),
    qtdFilmes: function() {
      return this.filmesSelecionados.length;
    }
  },
  methods: {
    meuCampeonato: async function() {
      try {
        let resposta = await axios.post(`${this.urlCopaFilmles}/Resultado`,this.filmesSelecionados);
        this.campeoes = resposta.data
      } catch (e) {
        this.$store.dispatch("erroServico", e)
      }
    }
  },
  mounted() {
    if (this.qtdFilmes == 8) {
      this.meuCampeonato()
    }
    else {
      this.$router.push('/')
    }
  }
}
</script>
