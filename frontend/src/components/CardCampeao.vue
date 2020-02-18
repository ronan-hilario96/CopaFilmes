<template>
    <v-card class="mx-auto d-flex card">
        <v-row class="bloco">
            <v-col class="posicao conteudo">{{posicao}}</v-col>
            <v-col  class="titulo conteudo">{{titulo}}</v-col>
        </v-row>
    </v-card>
</template>

<script>
export default {
  data: () => ({}),
  props: ["posicao", "titulo"],
  methods: {
  },
  mounted() {
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
.card {
    overflow: hidden;
    margin: 15px 0;
}
.card > .bloco > .conteudo {
    font-size: 53px;
}
.card > .bloco > .posicao {
    background: #c3c3c3;
    text-align: center;
    max-width: 100px;
}
</style>
