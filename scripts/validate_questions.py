import os
import json
import logging

logging.basicConfig(level=logging.DEBUG)

for filename in os.listdir("data/questions"):
    logging.info("Analyzing " + filename)
    with open(os.path.join("data/questions", filename), 'r') as f:
        text = f.read()
        try:
            data = json.loads(text)

            for q in data:
                if type(q["quest"]) is not str or type(q["image"]) is not str or type(q["answers"]) is not list or type(q["correct"]) is not int:
                    raise Exception(str(data.index(q)) + ": Some parameters are null, missing or their type is wrong.")

                if q["quest"] == "" and q["image"] == "":
                    raise Exception(str(data.index(q)) + ") Question's text and image cannot both be empty.")

                if len(q["answers"]) == 0:
                    raise Exception(str(data.index(q)) + ": Question has no answers.")

                for a in q["answers"]:
                    if type(a["text"]) is not str or type(a["image"]) is not str:
                        raise Exception(str(data.index(q)) + ": Some answer's parameters are null, missing or their type is wrong.")

                    if a["text"] == "" and a["image"] == "":
                        raise Exception(str(data.index(q)) + ": Answer's text and image cannot both be empty.")

        except Exception as e:
            logging.error(getattr(e, 'message', repr(e)))
            logging.fatal(filename + " is invalid. Aborting.")
            exit(1)

logging.info("Parsing successful!")
exit(0)