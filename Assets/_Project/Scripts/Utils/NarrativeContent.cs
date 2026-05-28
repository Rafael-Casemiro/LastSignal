using UnityEditor;
using UnityEngine;
using LastSignal.Data;

// Utilitário de editor: gera todos os MessageData assets narrativos do jogo.
// Menu: LastSignal > Generate Narrative Content
#if UNITY_EDITOR
namespace LastSignal.Utils
{
    public static class NarrativeContent
    {
        private const string FOLDER = "Assets/LastSignal/ScriptableObjects/Messages";

        [MenuItem("LastSignal/Generate Narrative Content")]
        public static void Generate()
        {
            EnsureFolder();

            // ─── MILITARES ────────────────────────────────────────────
            CreateMsg("MSG_Mil_01", "CORONEL VOSS", "CORONEL VOSS", LastSignal.Data.MessageType.Negotiation,
                "Coronel Voss, Setor Leste. Precisamos de combustível. Em troca: proteção militar por 3 dias. Aceita?",
                Choice("Aceitar o acordo (−30 Comb, +rep Militar)",
                    C(ConsequenceType.LoseResource, "Fuel", 30)),
                Choice("Recusar. Não confiamos em militares.",
                    C(ConsequenceType.ChangeReputation, "Military", -10)));

            CreateMsg("MSG_Mil_02", "CORONEL VOSS", "CORONEL VOSS", LastSignal.Data.MessageType.Negotiation,
                "Último aviso. Entregue os suprimentos ou declaramos o seu bunker território hostil.",
                Choice("Ceder. Enviar suprimentos. (−50 Comida, −20 Água)",
                    C(ConsequenceType.LoseResource, "Food", 50),
                    C(ConsequenceType.LoseResource, "Water", 20),
                    C(ConsequenceType.ChangeReputation, "Military", 15)),
                Choice("Resistir. Cortar comunicação.",
                    C(ConsequenceType.ChangeReputation, "Military", -30)));

            CreateMsg("MSG_Mil_03", "TENENTE ARIS", "TEN. ARIS", LastSignal.Data.MessageType.Story,
                "Aris aqui. O Coronel não sabe que estou transmitindo. Há um traidor no Setor Leste. Cuidado.",
                Choice("Agradecer a informação.",
                    C(ConsequenceType.ChangeReputation, "Military", 5)),
                Choice("Ignorar. Pode ser armadilha.",
                    C(ConsequenceType.ChangeReputation, "Military", 0)));

            // ─── REFUGIADOS ───────────────────────────────────────────
            CreateMsg("MSG_Ref_01", "VOZ_COLETIVA", "REPRESENTANTE DOS REFUGIADOS", LastSignal.Data.MessageType.Negotiation,
                "Somos 31 pessoas. Crianças, idosos. Não pedimos muito. Só um lugar para dormir esta noite.",
                Choice("Abrir as portas. Compartilhar o que temos.",
                    C(ConsequenceType.LoseResource, "Food", 40),
                    C(ConsequenceType.LoseResource, "Water", 20),
                    C(ConsequenceType.ChangeReputation, "Refugees", 25)),
                Choice("Oferecer suprimentos para 3 dias e orientar para outro abrigo.",
                    C(ConsequenceType.LoseResource, "Food", 20),
                    C(ConsequenceType.ChangeReputation, "Refugees", 10)),
                Choice("Negar acesso. Os recursos são insuficientes.",
                    C(ConsequenceType.ChangeReputation, "Refugees", -20)));

            CreateMsg("MSG_Ref_02", "VOZ_COLETIVA", "REPRESENTANTE DOS REFUGIADOS", LastSignal.Data.MessageType.Event,
                "Os refugiados que você acolheu encontraram um depósito abandonado de medicamentos. Estão enviando parte para vocês.",
                Choice("Aceitar gratamente.",
                    C(ConsequenceType.GainResource, "Medicine", 30),
                    C(ConsequenceType.ChangeReputation, "Refugees", 10)));

            CreateMsg("MSG_Ref_03", "VOZ_ANONIMA", "TRANSMISSÃO ANÔNIMA", LastSignal.Data.MessageType.Event,
                "Os refugiados que você rejeitou morreram na tempestade. Quatorze deles. Incluindo as crianças.",
                Choice("Registrar. Manter o protocolo.",
                    C(ConsequenceType.ChangeReputation, "Refugees", -15)));

            // ─── COMERCIANTES ─────────────────────────────────────────
            CreateMsg("MSG_Trade_01", "MARCIO_S", "MÁRCIO — REDE SUL", LastSignal.Data.MessageType.Negotiation,
                "Tenho 80 unidades de água purificada. Preço: 400 créditos. Entrega imediata.",
                Choice("Comprar. (−400 CR, +80 Água)",
                    C(ConsequenceType.LoseResource, "Credits", 400),
                    C(ConsequenceType.GainResource,  "Water", 80),
                    C(ConsequenceType.ChangeReputation, "Traders", 5)),
                Choice("Negociar. Oferecer 280 créditos.",
                    C(ConsequenceType.LoseResource, "Credits", 280),
                    C(ConsequenceType.GainResource,  "Water", 50),
                    C(ConsequenceType.ChangeReputation, "Traders", -5)),
                Choice("Recusar.",
                    C(ConsequenceType.ChangeReputation, "Traders", 0)));

            CreateMsg("MSG_Trade_02", "MARCIO_S", "MÁRCIO — REDE SUL", LastSignal.Data.MessageType.Negotiation,
                "Escuta. Alguém está comprando TUDO no mercado. Preços vão triplicar amanhã. Compre agora ou se arrependa.",
                Choice("Comprar estoque de comida emergencial. (−500 CR, +60 Comida)",
                    C(ConsequenceType.LoseResource, "Credits", 500),
                    C(ConsequenceType.GainResource,  "Food", 60)),
                Choice("Ignorar o alerta. Parece manipulação de preço.",
                    C(ConsequenceType.ChangeReputation, "Traders", -5)));

            CreateMsg("MSG_Trade_03", "NETWORK_S", "REDE SUL — BOLETIM", LastSignal.Data.MessageType.System,
                "MERCADO: Crise de combustível elevou preços 60% na região. Estoques críticos em 72h.",
                Choice("Registrar alerta.",
                    C(ConsequenceType.GainResource, "Credits", 0)));

            // ─── COALIZÃO NORTE ───────────────────────────────────────
            CreateMsg("MSG_Coal_01", "COORD_NORTE", "COORDENADOR DA COALIZÃO", LastSignal.Data.MessageType.Negotiation,
                "A Coalizão Norte está formando uma rede de bunkers cooperativos. Queremos você. Mas precisamos de compromisso.",
                Choice("Juntar-se à Coalizão. (Compartilhar recursos futuramente)",
                    C(ConsequenceType.ChangeReputation, "NorthCoalition", 20)),
                Choice("Manter independência.",
                    C(ConsequenceType.ChangeReputation, "NorthCoalition", -5)));

            CreateMsg("MSG_Coal_02", "COORD_NORTE", "COALIZÃO NORTE", LastSignal.Data.MessageType.Event,
                "Como membro da Coalizão, receba este suporte de emergência. Vocês merecem.",
                Choice("Aceitar o suporte.",
                    C(ConsequenceType.GainResource, "Food", 30),
                    C(ConsequenceType.GainResource, "Water", 25),
                    C(ConsequenceType.GainResource, "Medicine", 15)));

            // ─── REDE SUBTERRÂNEA ─────────────────────────────────────
            CreateMsg("MSG_Under_01", "PHANTASM", "PHANTASM — REDE UNDERGROUND", LastSignal.Data.MessageType.Negotiation,
                "Você não nos conhece. Mas sabemos tudo sobre você. Temos informações que podem salvar seu bunker. Preço: 300 créditos.",
                Choice("Pagar pela informação. (−300 CR)",
                    C(ConsequenceType.LoseResource, "Credits", 300),
                    C(ConsequenceType.ChangeReputation, "UndergroundNetwork", 15)),
                Choice("Recusar. Parece golpe.",
                    C(ConsequenceType.ChangeReputation, "UndergroundNetwork", -10)));

            CreateMsg("MSG_Under_02", "PHANTASM", "PHANTASM", LastSignal.Data.MessageType.Event,
                "A informação que você pagou: o Coronel Voss está planejando uma incursão ao seu bunker. Você tem 2 dias.",
                Choice("Preparar defesas. Reforçar portas.",
                    C(ConsequenceType.LoseResource, "Energy", 20)),
                Choice("Tentar negociar com Voss antes.",
                    C(ConsequenceType.ChangeReputation, "Military", 10)));

            // ─── EVENTOS AMBIENTAIS ───────────────────────────────────
            CreateMsg("MSG_Env_01", "SISTEMA", "MONITOR AMBIENTAL", LastSignal.Data.MessageType.System,
                "TEMPESTADE DE ÁCIDO detectada. Ventilação externa comprometida. Consumo de energia aumentará 40% por 2 dias.",
                Choice("Ativar filtros de emergência. (−30 Energia)",
                    C(ConsequenceType.LoseResource, "Energy", 30)),
                Choice("Desligar ventilação parcial. Risco para a saúde.",
                    C(ConsequenceType.LoseResource, "Medicine", 15)));

            CreateMsg("MSG_Env_02", "SISTEMA", "SENSOR SÍSMICO", LastSignal.Data.MessageType.System,
                "Atividade sísmica leve detectada a 4km. Infraestrutura do bunker em 87% de integridade.",
                Choice("Inspecionar estruturas. Custo: 2 dias de trabalho.",
                    C(ConsequenceType.LoseResource, "Credits", 50)));

            CreateMsg("MSG_Env_03", "SISTEMA", "MONITOR HÍDRICO", LastSignal.Data.MessageType.Event,
                "POSITIVO: Sensor detectou depósito de água subterrâneo a 800m. Perfuração possível.",
                Choice("Perfurar. (−100 CR, +150 Água em 2 dias)",
                    C(ConsequenceType.LoseResource, "Credits", 100),
                    C(ConsequenceType.GainResource, "Water", 150)),
                Choice("Investigar antes de investir.",
                    C(ConsequenceType.ChangeReputation, "UndergroundNetwork", 0)));

            // ─── TRANSMISSÕES MISTERIOSAS ─────────────────────────────
            CreateMsg("MSG_Myst_01", "???", "TRANSMISSÃO NÃO IDENTIFICADA", LastSignal.Data.MessageType.Story,
                "... sobreviventes do Bunker Zeta... colapso iminente... se alguém ouvir... ajuda...",
                Choice("Tentar localizar a origem.",
                    C(ConsequenceType.LoseResource, "Energy", 10)),
                Choice("Ignorar. Pode ser armadilha.",
                    C(ConsequenceType.ChangeReputation, "Refugees", -5)));

            CreateMsg("MSG_Myst_02", "???", "FREQUÊNCIA DESCONHECIDA", LastSignal.Data.MessageType.Story,
                "O colapso não foi um acidente. Há algo além da zona de quarentena. Não confie nos relatórios oficiais.",
                Choice("Arquivar e investigar.",
                    C(ConsequenceType.GainResource, "Credits", 0)));

            CreateMsg("MSG_Myst_03", "ATLAS", "ATLAS — IDENTIDADE DESCONHECIDA", LastSignal.Data.MessageType.Negotiation,
                "Existe um bunker seguro no setor norte. Tenho as coordenadas. Posso enviar. Mas isso tem um preço.",
                Choice("Pagar 500 créditos pelas coordenadas.",
                    C(ConsequenceType.LoseResource, "Credits", 500),
                    C(ConsequenceType.TriggerEnding, "Survived", 0)),
                Choice("Recusar. Provavelmente mentira.",
                    C(ConsequenceType.ChangeReputation, "UndergroundNetwork", 0)));

            // ─── EVENTOS INTERNOS ─────────────────────────────────────
            CreateMsg("MSG_Int_01", "SISTEMA", "RELATÓRIO INTERNO", LastSignal.Data.MessageType.Event,
                "Conflito entre grupos dentro do bunker. Dois sobreviventes feriram-se. Mediação necessária.",
                Choice("Mediar o conflito. Distribuir responsabilidades.",
                    C(ConsequenceType.LoseResource, "Medicine", 5)),
                Choice("Impor lei marcial temporária.",
                    C(ConsequenceType.ChangeReputation, "Military", 10),
                    C(ConsequenceType.ChangeReputation, "Refugees", -15)));

            CreateMsg("MSG_Int_02", "SISTEMA", "RELATÓRIO MÉDICO", LastSignal.Data.MessageType.Event,
                "Três sobreviventes com sintomas de contaminação leve. Medicamentos podem controlar.",
                Choice("Tratar imediatamente. (−20 Medicina)",
                    C(ConsequenceType.LoseResource, "Medicine", 20)),
                Choice("Quarentena e racionamento de medicamentos.",
                    C(ConsequenceType.LoseResource, "Medicine", 8)));

            CreateMsg("MSG_Int_03", "SISTEMA", "RELATÓRIO DE PRODUÇÃO", LastSignal.Data.MessageType.Event,
                "Sobreviventes organizaram horta interna. Produção mínima mas constante de alimentos.",
                Choice("Expandir horta. (−30 CR, +5 Comida/dia)",
                    C(ConsequenceType.LoseResource, "Credits", 30),
                    C(ConsequenceType.GainResource, "Food", 20)));

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("Conteúdo narrativo gerado: 22 MessageData assets.");
        }

        static void EnsureFolder()
        {
            if (!AssetDatabase.IsValidFolder(FOLDER))
                AssetDatabase.CreateFolder("Assets/LastSignal/ScriptableObjects", "Messages");
        }

        static void CreateMsg(string id, string senderId, string displayName,
            LastSignal.Data.MessageType type, string text, params ChoiceData[] choices)
        {
            string path = $"{FOLDER}/{id}.asset";
            if (AssetDatabase.LoadAssetAtPath<MessageData>(path) != null) return;

            var msg = ScriptableObject.CreateInstance<MessageData>();
            msg.senderId          = senderId;
            msg.senderDisplayName = displayName;
            msg.messageType       = type;
            msg.messageText       = text;
            msg.choices           = choices;
            msg.triggerChance     = 1f;
            AssetDatabase.CreateAsset(msg, path);
        }

        static ChoiceData Choice(string text, params ChoiceConsequence[] cons)
            => new ChoiceData { choiceText = text, consequences = cons };

        static ChoiceConsequence C(ConsequenceType t, string target, int val)
            => new ChoiceConsequence { type = t, targetId = target, value = val };
    }
}
#endif
