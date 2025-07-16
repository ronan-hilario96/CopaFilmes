<template>
  <v-card 
    :class="{ 'filme-selecionado': selecionado, 'filme-disabled': !podeSelecionar }"
    class="filme-card mx-auto d-flex align-center pa-4"
    @click="toggleSelecao"
    :elevation="selecionado ? 8 : 2"
    :disabled="loading"
  >
    <v-checkbox 
      v-model="selecionado" 
      @click.stop="toggleSelecao"
      :disabled="!podeSelecionar || loading"
      color="primary"
      hide-details
    />
    
    <v-card-text class="flex-grow-1 pa-0 ml-3">
      <div class="filme-info">
        <h3 class="filme-titulo">{{ titulo }}</h3>
        <p class="filme-ano text--secondary">{{ ano }}</p>
        
        <div class="filme-meta d-flex align-center">
          <v-icon small color="amber">mdi-star</v-icon>
          <span class="ml-1 font-weight-medium">{{ nota }}</span>
        </div>
      </div>
    </v-card-text>

    <v-fade-transition>
      <v-progress-circular
        v-if="loading"
        indeterminate
        color="primary"
        size="20"
        width="2"
      />
    </v-fade-transition>
  </v-card>
</template>

<script>
import { mapState, mapGetters, mapActions } from "vuex";

export default {
  name: 'Filme',
  
  props: {
    id: {
      type: String,
      required: true
    },
    titulo: {
      type: String,
      required: true
    },
    ano: {
      type: Number,
      required: true
    },
    nota: {
      type: Number,
      required: true
    }
  },

  data() {
    return {
      loading: false
    };
  },

  computed: {
    ...mapState(['filmesSelecionados', 'config']),
    ...mapGetters(['temFilmeSelecionado']),

    selecionado: {
      get() {
        return this.temFilmeSelecionado(this.id);
      },
      set(value) {
        // Handled by toggleSelecao method
      }
    },

    podeSelecionar() {
      return this.selecionado || this.filmesSelecionados.length < this.config.maxFilmesSelecionados;
    },

    filmeData() {
      return {
        id: this.id,
        titulo: this.titulo,
        ano: this.ano,
        nota: this.nota
      };
    }
  },

  methods: {
    ...mapActions(['adicionarFilme', 'removerFilme']),

    async toggleSelecao() {
      if (this.loading) return;

      this.loading = true;

      try {
        if (this.selecionado) {
          await this.removerFilme(this.filmeData);
        } else {
          const sucesso = await this.adicionarFilme(this.filmeData);
          if (!sucesso) {
            // Feedback visual se não conseguiu adicionar
            this.$emit('erro-selecao', 'Limite máximo de filmes atingido');
          }
        }
      } catch (error) {
        console.error('Erro ao alterar seleção:', error);
        this.$emit('erro-selecao', 'Erro ao alterar seleção do filme');
      } finally {
        // Small delay for better UX
        setTimeout(() => {
          this.loading = false;
        }, 200);
      }
    }
  }
};
</script>

<style scoped>
.filme-card {
  transition: all 0.3s ease;
  cursor: pointer;
  border: 2px solid transparent;
  min-height: 100px;
}

.filme-card:hover:not(.filme-disabled) {
  transform: translateY(-2px);
  box-shadow: 0 8px 25px rgba(0, 0, 0, 0.15) !important;
}

.filme-selecionado {
  border-color: #1976d2;
  background-color: rgba(25, 118, 210, 0.05);
}

.filme-disabled {
  opacity: 0.6;
  cursor: not-allowed;
}

.filme-disabled:hover {
  transform: none !important;
  box-shadow: none !important;
}

.filme-info {
  display: flex;
  flex-direction: column;
  gap: 4px;
}

.filme-titulo {
  font-size: 1.1rem;
  font-weight: 500;
  line-height: 1.3;
  margin-bottom: 0;
  color: #2c3e50;
}

.filme-ano {
  font-size: 0.9rem;
  margin-bottom: 0;
  font-weight: 400;
}

.filme-meta {
  margin-top: 8px;
}

.filme-meta span {
  font-size: 0.9rem;
  color: #666;
}

/* Loading state */
.filme-card:disabled {
  opacity: 0.8;
  cursor: progress;
}

/* Responsive */
@media (max-width: 600px) {
  .filme-card {
    min-height: 80px;
  }
  
  .filme-titulo {
    font-size: 1rem;
  }
  
  .filme-ano {
    font-size: 0.8rem;
  }
}
</style>
