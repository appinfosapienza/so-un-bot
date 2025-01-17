# WARNING
**We are working on a completely new version of the bot, written in Python. Stay tuned!**

---
# so-un-bot
The official https://t.me/so_1_bot questions repository

🇮🇹 Raccolta di domande con risposta multipla utili per esercitarsi in molteplici esami!

### Struttura del repository

In Data/Questions sono presenti tutte le domande attualmente presenti nel bot, il nome del file corrisponde al nome del comando sul bot.
Per aggiungere o correggere domande potete fare una Pull Request a questa repo.

In Utils trovate script, sviluppati da vari studenti del corso, per creare o validare i file delle domande.

**Nota per gli admin di appinfosapienza:**
Al momento non sono presenti dei test CI che testano l'integrità del repository prima di un deploy.
Quando accettate una Pull Request, entro due minuti verrà lanciata una nuova build sul server di produzione e al termine eseguito il bot con la nuova versione.

Non essendoci test CI, se sono presenti errori, un commit contenente errori può mandare offline il bot (ad es. se il bot non riesce a fare il parsing di tutte le domande all'avvio).

**Per i contributori:** 
### Struttura dei file

Il bot accetta le domande sia in un singolo file (utilizzato ad esesempio da Sistemi Operativi 1 e 2), che in file multipli (utilizzato da Ingegneria del Software).
È in programma l'implementazione del supporto al formato JSON.

#### Singolo file

- Le domande sono separate da una riga vuota
- Le risposte possono essere su una riga sola
- Le risposte devono iniziano per `> ` se sono errate, per `v ` se sono corrette
- Non ci possono essere righe vuote nella stessa domanda
- Solo la domanda può contenere un'immagine, non le risposte

#### File multipli

- Ogni domanda è in una directory separata
- La directory ha come nome il numero della domanda
- Il testo della domanda è nel file `quest.txt`
- La risposta corretta è nel file `correct.txt`
- Gli altri file contengono solo risposte errate
- Sia domande che risposte possono contenere immagini (max un'immagine per domanda e una per ciascuna risposta)
