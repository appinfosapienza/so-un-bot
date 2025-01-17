### Moodle Scraper

Web scraper in Python per salvare un quiz Moodle in un formato compatibile con il bot.

Per far funzionare lo scraper, è necessario creare un file .env nella directory dello scraper e inserire rispettivamente:

USER = Matricola
PASSWORD = Password di Infostud

Infine inserire il chromedriver sempre nella directory dello scraper.

Una volta lanciato lo script, le domande verranno inserite formattate in cartelle che verranno create all'interno della cartella Scraper. Fare attenzione al fatto che le domande con immagini non sono gestite e dovranno comunque essere inserite a mano. Non sono gestiti nemmeno le parti di codice, quindi il tag "pre" dovrà essere inserito a mano.
