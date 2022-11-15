from selenium import webdriver
from selenium.webdriver.common.by import By
from dotenv import load_dotenv
import time
import os

load_dotenv()


matricola = os.getenv('UTENTE')
password = os.getenv('PASSWORD')


url_risultati_esame = "https://elearning.uniroma1.it/mod/quiz/review.php?attempt=1096076&cmid=385932"
codice_esame = "0321"


# Main Function
if __name__ == '__main__':
  
    options = webdriver.ChromeOptions()
    options.add_argument("--start-maximized")
    options.add_argument('--log-level=3')
  
    # Provide the path of chromedriver present on your system.
    driver = webdriver.Chrome(executable_path="./chromedriver.exe",
                              chrome_options=options)
    driver.set_window_size(1920,1080)
  
    # Loggo in elearning
    driver.get('https://elearning.uniroma1.it/auth/mtsaml/')
    time.sleep(1)
    userbox = driver.find_element(By.ID, "username")
    passbox = driver.find_element(By.ID, "password")
    userbox.send_keys(matricola)
    passbox.send_keys(password)
    button = driver.find_element(By.NAME, "_eventId_proceed")
    button.click()
    time.sleep(1)

    #Apro i risultati del test
    driver.get(url_risultati_esame)

    # Ottengo tutti i box delle domande
    qboxes = driver.find_elements(By.CLASS_NAME, "qtext")

    #Prendo tutti i box delle risposte e li divido (n.b. controllare nei risultati che la divisione sia corretta)
    answers = driver.find_elements(By.CLASS_NAME, "answer")
    answers_cleaned = []
    for i, answer in enumerate(answers):
        with open("risposte.txt", "w") as f1:
            f1.write(answer.text)
        answers_cleaned.append([])
        answers_cleaned[i].append(answer.text.split("1.",1)[1].split("2.",1)[0])
        answers_cleaned[i].append(answer.text.split("1.",1)[1].split("2.",1)[1].split("3.",1)[0])
        answers_cleaned[i].append(answer.text.split("1.",1)[1].split("2.",1)[1].split("3.",1)[1])

    # Ottenfo tutti i box delle risposte corrette e elimino la prima parte che non è necessaria
    rightAnswers = driver.find_elements(By.CLASS_NAME, "rightanswer")
    right = []
    for ranswer in rightAnswers:
        right.append(ranswer.text[24:])


    #Procedo alla creazione dei file
    i = 0    
    for domanda, risposte, corretta in zip(qboxes, answers_cleaned, right):
        cartella = codice_esame + "_" + str(i)
        os.mkdir(cartella)
        with open(cartella + "/quest.txt", "w") as f:
            f.write(domanda.text.strip())
        j=1
        for risp in risposte:
            #Se la risposta coincide con quella corretta la metto in correct.txt altrimenti va in un file wrong{indice}.txt
            #N.B. Il controlla a volte non funziona correttamente (vengono generati solo file wrong.txt)
            if (risp.strip() == corretta.strip()):
                with open(cartella + "/correct.txt", "w") as f:
                    f.write(risp.strip())
            else:
                with open(cartella + "/wrong"+str(j)+".txt", "w") as f:
                    f.write(risp.strip())
                j = j+1
        i = i+1

    time.sleep(60)
    driver.quit()
    print("Done")