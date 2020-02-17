<template>
<v-app>
    <v-content>
      <router-view/>
    </v-content>

    <v-bottom-sheet v-model="erroServico" persistent>
      <v-sheet class="text-center" height="200px">
        <v-btn class="mt-6" flat color="error" @click="tentarNovamente">Tentar Novamente</v-btn>
        <div class="py-6 display-3" >Serviço fora do ar</div>
      </v-sheet>
    </v-bottom-sheet>
  </v-app>
</template>


<script>
import { mapState } from "vuex";

export default {
  computed: {
    ...mapState(["erroServico"]),
  },
  methods: {
    tentarNovamente: function () {
      this.$store.dispatch("obterFilmes")
    }
  },
}
</script>
