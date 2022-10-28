# so-un-bot
The official https://t.me/so_1_bot questions repository

🇮🇹 🇮🇹 🇮🇹

### Struttura dei file

Il bot accetta le domande sia in un singolo file (utilizzato da Sistemi Operativi 1 e 2), che in file multipli (utilizzato da Ingegneria del Software)

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

### Immagini

Per risparmaire traffico dati sul server, le immagini devono essere caricate su un servizio esterno (es. Imgur).
Per inserire un'immagine basta aggiungere una riga contenente solo `img=<url immagine>` all'inizio della domanda o risposta.
Nella struttura a file multipli, deve essere la prima riga del file, e il resto del file verrà aggiunto come didascalia.
