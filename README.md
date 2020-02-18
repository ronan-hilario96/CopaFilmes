# A Copa do Mundo de Filmes

A Copa do mundo de filmes funciona da seguinte maneira: uma lista inicial de 8 filmes é selecionada e,
dentre esses filmes, são realizadas partidas para encontrar o vencedor. Para montar o chaveamento
será necessário:

* Ordenar os filmes alfabeticamente;
* Fazer com que os filmes disputem em eliminatórias da seguinte forma: o filme na primeira
posição disputará contra o da última posição, o segundo com o penúltimo, e assim por diante

* Fase de Eliminatórias
    * Dessa fase em diante é necessário realizar partidas entre os filmes. Partida é uma disputa entre dois
filmes, onde o filme com maior nota é o vencedor. Caso existam disputas entre filmes com mesma nota,
utilize o critério de desempate descrito neste arquivo mais abaixo.
O chaveamento é definido da seguinte maneira: Vencedor da disputa entre Filme 1 e Filme 8 enfrenta o
vencedor da disputa do Filme 2 com o Filme 7, enquanto o vencedor da disputa entre o Filme 3 e o Filme
6 enfrenta o vencedor da disputa entre Filme 4 e Filme 5. Depois esses vencedores disputarão entre si,
sendo o primeiro vencedor contra o segundo vencedor e o terceiro vencedor contra o quarto. Os dois
filmes restantes disputam entre si para encontrar o campeão. Para uma melhor visualização de como deve
ser a competição.

## Back

Projetos backend e de teste

### Backend

Versão utilizado do .net core: **3.1.100**

```bash
dotnet run
```

#### Endpoint

Obtém uma lista de filmes

> GET: v1/CopaFilmes/Filmes

Envia uma lista de filmes e retorna dois ganhadores

> POST: v1/CopaFilmes/Resultado

### TesteCopaFilmes

Xunit

```bash
dotnet test
```

## Fron

Versão vue cli: **@vue/cli 4.1.2**

Versão node: **v12.14.0**

Versão vuetify **2.0.5**

Necessário para rodar o front

1. Instalações
    instalar node v12.14.0
    ```bash
    npm install -g @vue/cli
    ```
    Instalar dependências do projeto
    ```bash
    npm install
    ```

2. Modificar endpoint das requisições 
    > src\store\index.js _urlCopaFilmles: "https://localhost:5001/v1/CopaFilmes"_

3. Executar o projeto
    ```bash
    npm run serve
    ```