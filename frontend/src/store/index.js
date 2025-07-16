import Vue from 'vue'
import Vuex from 'vuex'
import axios from 'axios'

Vue.use(Vuex)

export default new Vuex.Store({
  state: {
    filmes: [],
    filmesSelecionados: [],
    resultadoCampeonato: null,
    loading: {
      filmes: false,
      campeonato: false
    },
    error: {
      message: null,
      type: null
    },
    config: {
      urlCopaFilmes: process.env.VUE_APP_API_URL || "https://localhost:44306/v1/CopaFilmes",
      maxFilmesSelecionados: 8
    }
  },
  
  mutations: {
    SET_FILMES(state, filmes) {
      state.filmes = filmes
    },
    
    SET_LOADING(state, { key, value }) {
      Vue.set(state.loading, key, value)
    },
    
    SET_ERROR(state, { message, type }) {
      state.error = { message, type }
    },
    
    CLEAR_ERROR(state) {
      state.error = { message: null, type: null }
    },
    
    ADICIONAR_FILME(state, filme) {
      if (state.filmesSelecionados.length < state.config.maxFilmesSelecionados) {
        const jaExiste = state.filmesSelecionados.some(f => f.id === filme.id)
        if (!jaExiste) {
          state.filmesSelecionados.push(filme)
        }
      }
    },
    
    REMOVER_FILME(state, filme) {
      state.filmesSelecionados = state.filmesSelecionados.filter(f => f.id !== filme.id)
    },
    
    LIMPAR_SELECAO(state) {
      state.filmesSelecionados = []
    },
    
    SET_RESULTADO_CAMPEONATO(state, resultado) {
      state.resultadoCampeonato = resultado
    },
    
    LIMPAR_RESULTADO_CAMPEONATO(state) {
      state.resultadoCampeonato = null
    }
  },
  
  actions: {
    async obterFilmes({ commit, state }) {
      try {
        commit('SET_LOADING', { key: 'filmes', value: true })
        commit('CLEAR_ERROR')
        
        const response = await axios.get(`${state.config.urlCopaFilmes}/Filmes`)
        
        if (response.data && Array.isArray(response.data)) {
          commit('SET_FILMES', response.data)
        } else {
          throw new Error('Formato de resposta inválido')
        }
      } catch (error) {
        console.error('Erro ao obter filmes:', error)
        
        let errorMessage = 'Erro ao carregar filmes'
        let errorType = 'error'
        
        if (error.response) {
          // Erro da API
          if (error.response.status === 503) {
            errorMessage = 'Serviço temporariamente indisponível. Tente novamente mais tarde.'
            errorType = 'warning'
          } else if (error.response.status >= 500) {
            errorMessage = 'Erro interno do servidor. Tente novamente mais tarde.'
          } else if (error.response.status >= 400) {
            errorMessage = 'Erro na requisição. Verifique sua conexão.'
          }
        } else if (error.request) {
          // Erro de rede
          errorMessage = 'Erro de conexão. Verifique sua internet.'
          errorType = 'warning'
        }
        
        commit('SET_ERROR', { message: errorMessage, type: errorType })
        throw error
      } finally {
        commit('SET_LOADING', { key: 'filmes', value: false })
      }
    },
    
    async executarCampeonato({ commit, state }) {
      try {
        commit('SET_LOADING', { key: 'campeonato', value: true })
        commit('CLEAR_ERROR')
        commit('LIMPAR_RESULTADO_CAMPEONATO')
        
        if (state.filmesSelecionados.length !== state.config.maxFilmesSelecionados) {
          throw new Error(`Selecione exatamente ${state.config.maxFilmesSelecionados} filmes para o campeonato`)
        }
        
        const response = await axios.post(`${state.config.urlCopaFilmes}/Resultado`, state.filmesSelecionados)
        
        if (response.data && Array.isArray(response.data) && response.data.length >= 2) {
          commit('SET_RESULTADO_CAMPEONATO', {
            campeao: response.data[0],
            viceCampeao: response.data[1],
            timestamp: new Date()
          })
        } else {
          throw new Error('Formato de resposta inválido')
        }
      } catch (error) {
        console.error('Erro ao executar campeonato:', error)
        
        let errorMessage = 'Erro ao executar campeonato'
        
        if (error.response && error.response.data) {
          errorMessage = error.response.data.detail || error.response.data.message || errorMessage
        } else if (error.message) {
          errorMessage = error.message
        }
        
        commit('SET_ERROR', { message: errorMessage, type: 'error' })
        throw error
      } finally {
        commit('SET_LOADING', { key: 'campeonato', value: false })
      }
    },
    
    adicionarFilme({ commit, state }, filme) {
      if (state.filmesSelecionados.length >= state.config.maxFilmesSelecionados) {
        commit('SET_ERROR', {
          message: `Você já selecionou o máximo de ${state.config.maxFilmesSelecionados} filmes`,
          type: 'warning'
        })
        return false
      }
      
      commit('ADICIONAR_FILME', filme)
      commit('CLEAR_ERROR')
      return true
    },
    
    removerFilme({ commit }, filme) {
      commit('REMOVER_FILME', filme)
      commit('CLEAR_ERROR')
    },
    
    limparSelecao({ commit }) {
      commit('LIMPAR_SELECAO')
      commit('CLEAR_ERROR')
    },
    
    limparError({ commit }) {
      commit('CLEAR_ERROR')
    },
    
    reiniciarCampeonato({ commit }) {
      commit('LIMPAR_RESULTADO_CAMPEONATO')
      commit('LIMPAR_SELECAO')
      commit('CLEAR_ERROR')
    }
  },
  
  getters: {
    filmesDisponiveis: (state) => {
      return state.filmes.filter(filme => 
        !state.filmesSelecionados.some(selecionado => selecionado.id === filme.id)
      )
    },
    
    podeIniciarCampeonato: (state) => {
      return state.filmesSelecionados.length === state.config.maxFilmesSelecionados
    },
    
    temFilmeSelecionado: (state) => (filmeId) => {
      return state.filmesSelecionados.some(filme => filme.id === filmeId)
    },
    
    contadorSelecao: (state) => {
      return `${state.filmesSelecionados.length}/${state.config.maxFilmesSelecionados}`
    },
    
    isLoading: (state) => {
      return Object.values(state.loading).some(loading => loading)
    },
    
    hasError: (state) => {
      return state.error.message !== null
    }
  }
})