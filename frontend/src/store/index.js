import Vue from 'vue'
import Vuex from 'vuex'
import axios from 'axios'

Vue.use(Vuex)

export default new Vuex.Store({
  state: {
    filmes: [],
    filmesSelecionados: [],
    erroServico: null,
    urlCopaFilmles: "https://localhost:44306/v1/CopaFilmes"
  },
  mutations: {
    SET_FILMES(state, filmes) {
      state.filmes = filmes
      state.erroServico = null
    },
    adicionarFilme(state, filme) {
      if (state.filmesSelecionados.length < 8)
        state.filmesSelecionados.push(filme)
    },
    removerFilme(state, filme) {
      state.filmesSelecionados = state.filmesSelecionados.filter(x => x.id !== filme.id)
    },
    erroServico(state, servico) {
      state.erroServico = servico
    }
  },
  actions: {
    obterFilmes({ commit, state }) {
      axios
        .get(`${state.urlCopaFilmles}/Filmes`)
        .then(r => r.data)
        .then(data => commit('SET_FILMES', data))
        .catch(r => commit('erroServico', r))
    }
  },
  modules: {
  }
})