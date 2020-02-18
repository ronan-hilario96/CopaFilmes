<template>
  <v-container>
    <CardPrincipal titulo="CAMPEONATO DE FILMES" subtitulo="Fase de Seleção">
      <p>Selecione 8 filmes que você deseja que entrem na competição e depois precione o botão Gerar Meu Campeonato para prosseguir.</p>
    </CardPrincipal>
    <v-row align="center" justify="space-between">
      <v-col>
        <p>Selecionados</p>
        <p>{{qtdFilmes}} de 8 filmes</p>
      </v-col>
      <v-col class="text-right">
        <v-btn :disabled="qtdFilmes != 8" to="/resultado">Gerar Meu Campeonato</v-btn>
      </v-col>
    </v-row>
    <ListaFilmes />
  </v-container>
</template>

<script>
import CardPrincipal from "../components/CardPrincipal";
import ListaFilmes from "../components/ListaFilmes";
import { mapState } from "vuex";

export default {
  name: 'Home',
  components: {
    CardPrincipal,
    ListaFilmes
  },
  computed: {
    ...mapState(["filmesSelecionados"]),
    qtdFilmes: function() {
      return this.filmesSelecionados.length;
    }
  },
  methods: {
  },
}
</script>
