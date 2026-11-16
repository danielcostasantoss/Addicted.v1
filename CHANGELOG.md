# Changelog

## [0.1.0] — 2026-11-15

Primeira versão jogável (protótipo). Arte e regras provisórias.

### Adicionado
- Projeto Unity 6.3 LTS (6000.3.28f1) com URP e 2D Renderer, estrutura `Assets/_Project/` e Assembly Definitions.
- **Sala Escritório** (cena inicial): vista lateral com profundidade, estilo briga de rua.
  - Movimento em dois eixos dentro da faixa do chão, dash com rastro ciano e magenta.
  - Ciclo do vício: Rotina → Pico → Crash, e Limite cardíaco ao exagerar nas doses, com sprite por estado.
  - Golpe corpo a corpo com combo; inimigos perseguidor e atirador; duas ondas; saída que abre ao limpar a sala.
  - HUD de depuração: vício, pulso, estado, combo e ondas.
- Cena Sandbox com controlador alternativo de plataforma (pulo, coyote time, buffer de pulo, dash).
- Build Windows x64 (IL2CPP) pelo menu **Addicted → Build** ou pela linha de comando.
- Workflow do GitHub Actions para testes e build (GameCI).
- ADR-001 com a proposta de arquitetura.

### Limitações conhecidas
- O jogador não perde vida; levar um tiro só zera o combo.
- A saída abre, mas ainda não leva a outra sala.
- Não há menu, áudio, salvamento nem gatilhos de história.
