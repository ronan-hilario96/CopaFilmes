import Vue from 'vue'
import Vuex from 'vuex'
import axios from 'axios'

Vue.use(Vuex)

export default new Vuex.Store({
  state: {
    filmes: [{}],
    filmesSelecionados: []
  },
  mutations: {
    SET_FILMES(state, filmes) {
      state.filmes = filmes
    },
    adicionarFilme(state, filme) {
      if (state.filmesSelecionados.length < 8)
        state.filmesSelecionados.push(filme)
    },
    removerFilme(state, filme) {
      state.filmesSelecionados = state.filmesSelecionados.filter(x => x.id !== filme.id)
    }
  },
  actions: {
    obterFilmes({ commit }) {
      axios
        .get("https://localhost:44306/v1/CopaFilmes/Obter")
        .then(r => r.data)
        .then(data => commit('SET_FILMES', data))
    }
  },
  modules: {
  }
})