<template>
  <v-app>
    <v-app-bar app color="primary" dark>
      <v-toolbar-title>
        <v-icon left>mdi-trophy</v-icon>
        Copa do Mundo de Filmes
      </v-toolbar-title>
      <v-spacer></v-spacer>
      <v-btn text @click="reiniciarCampeonato" v-if="resultadoCampeonato">
        <v-icon left>mdi-refresh</v-icon>
        Novo Campeonato
      </v-btn>
    </v-app-bar>

    <v-main>
      <v-container>
        <!-- Loading Global -->
        <v-overlay :value="isLoading" color="primary">
          <v-progress-circular indeterminate size="64" color="white"></v-progress-circular>
          <div class="text-center mt-4 white--text">
            <h3>Carregando...</h3>
          </div>
        </v-overlay>

        <!-- Error Alert -->
        <v-alert
          v-if="hasError"
          :type="error.type || 'error'"
          dismissible
          @input="limparError"
          class="mb-4"
        >
          <v-row align="center">
            <v-col class="grow">
              {{ error.message }}
            </v-col>
            <v-col class="shrink">
              <v-btn
                text
                color="white"
                @click="tentarNovamente"
                :disabled="isLoading"
              >
                Tentar Novamente
              </v-btn>
            </v-col>
          </v-row>
        </v-alert>

        <!-- Content -->
        <router-view />
      </v-container>
    </v-main>

    <!-- Footer -->
    <v-footer app color="primary" dark>
      <v-row justify="center">
        <v-col class="text-center">
          <span>&copy; {{ new Date().getFullYear() }} Copa do Mundo de Filmes - Versão Melhorada</span>
        </v-col>
      </v-row>
    </v-footer>
  </v-app>
</template>

<script>
import { mapState, mapGetters, mapActions } from "vuex";

export default {
  name: 'App',
  
  computed: {
    ...mapState(['error', 'resultadoCampeonato']),
    ...mapGetters(['isLoading', 'hasError'])
  },

  methods: {
    ...mapActions(['obterFilmes', 'limparError', 'reiniciarCampeonato']),

    tentarNovamente() {
      this.limparError();
      this.obterFilmes();
    }
  },

  created() {
    // Carrega os filmes ao iniciar a aplicação
    this.obterFilmes();
  }
};
</script>

<style>
.v-application {
  font-family: 'Roboto', sans-serif;
}

.v-toolbar-title {
  font-weight: 500;
}

.v-main {
  min-height: 100vh;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
}

.v-container {
  max-width: 1200px;
}

.v-alert {
  border-radius: 8px;
}

.v-btn {
  text-transform: none;
  border-radius: 8px;
}

.v-card {
  border-radius: 12px;
}

.v-footer {
  font-size: 0.9rem;
}
</style>
