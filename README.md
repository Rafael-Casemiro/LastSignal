# Last Signal

Jogo de sobrevivência narrativa em Unity ambientado em um bunker pós-apocalíptico. O jogador gerencia recursos escassos, negocia com facções externas e toma decisões morais através de um terminal de comunicação retrô — cada escolha pode determinar o destino dos sobreviventes.

## Sobre o jogo

Você está no controle do **Bunker Alfa**. O mundo colapsou. Durante os dias que restam, transmissões chegam de facções que ainda existem lá fora: militares, refugiados, comerciantes, coalizões e redes clandestinas. Você lê, decide e responde. Os recursos diminuem a cada dia. As alianças oscilam. O silêncio também é uma escolha.

## Mecânicas principais

**Recursos**
- Comida, Água, Combustível, Medicina, Energia e Créditos
- Consumo diário automático — deixar Comida ou Água chegarem a zero encerra o jogo

**Facções**
- 5 grupos com reputação individual de -100 a +100
- Relações variam de `Inimigo` a `Aliado` e afetam preços de negociação e eventos disponíveis

| Facção             | Papel                              |
|--------------------|------------------------------------|
| Military           | Ordem e proteção, mas exige lealdade |
| Refugees           | Sobreviventes desesperados          |
| Traders            | Mercado negro de suprimentos        |
| NorthCoalition     | Aliança política instável           |
| UndergroundNetwork | Informações e rotas secretas        |

**Narrativa**
- Mensagens chegam em fila com animação de digitação no terminal
- Cada mensagem pode exibir escolhas com consequências diretas (ganhar/perder recursos, alterar reputação, desencadear um final)

**Finais possíveis**

| Final           | Condição de gatilho                                          |
|-----------------|--------------------------------------------------------------|
| Sobrevivência   | Chegar ao último dia com Food > 20 e Water > 15              |
| Sacrifício      | Reputação com Refugiados ≥ 70 com comida quase zerada        |
| Ditadura        | Reputação com Militares ≥ 80                                 |
| Isolamento      | 5 dias sem responder nenhuma mensagem                        |
| Colapso         | Recursos esgotados antes do prazo                            |
| Colapso Diplomático | Todas as facções hostis                                  |

## Estrutura do projeto

```
Assets/
├── _Project/
│   ├── Audio/              Sons de interface e ambiente
│   ├── Scenes/             Cena principal (MainMenu)
│   ├── ScriptableObjects/  Dados de mensagens e facções
│   └── Scripts/
│       ├── Core/           Managers centrais do jogo
│       ├── Data/           Estruturas de dados (MessageData)
│       ├── Editor/         Ferramentas do editor Unity
│       ├── Narrative/      Orquestração de mensagens e escolhas
│       ├── UI/             Interface do terminal e painéis
│       └── Utils/          Bootstrapper e constantes narrativas
└── LastSignal/
    ├── Prefabs/            Prefabs de UI (linhas, botões de escolha)
    ├── Scenes/             Cena do menu principal
    └── ScriptableObjects/  Base de dados de mensagens do jogo
```

**Scripts principais**

| Script | Responsabilidade |
|--------|-----------------|
| `GameManager` | Singleton central; coordena estado geral e game over |
| `ResourceManager` | Estoque de recursos com eventos de mudança |
| `TimeManager` | Progressão por dias e evento de colapso |
| `FactionManager` | Reputação, relações e multiplicadores de preço |
| `NarrativeManager` | Fila de mensagens, exibição e processamento de escolhas |
| `EndingManager` | Avaliação diária de condições de final |
| `MarketManager` | Comércio entre o jogador e as facções |
| `TerminalUI` | Impressão animada de linhas no terminal |
| `AudioManager` | Sons de notificação, teclado e ambiente |
| `SaveManager` | Persistência de estado do jogo |

## Tecnologias

- **Unity** (C#)
- **TextMesh Pro** — renderização de texto no terminal
- **Unity Input System** — entrada do jogador
- **ScriptableObjects** — dados de mensagens e eventos desacoplados do código

## Como abrir

1. Clone o repositório
2. Abra a pasta no **Unity Hub** (versão recomendada: a indicada em `ProjectSettings/ProjectVersion.txt`)
3. Aguarde a importação dos pacotes
4. Abra a cena `Assets/_Project/Scenes/MainMenu.unity`
5. Pressione Play
