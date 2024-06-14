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
                if q["quest"] == "" and q["image"] == "":
                    raise Exception(str(data.index(q)) + ") Question's text and image cannot both be empty.")
                if q["quest"] is None or q["image"] is None or q["answers"] is None or q["correct"] is None:
                    raise Exception(str(data.index(q)) + ") Some parameters are null or missing.")

                if len(q["answers"]) == 0:
                    raise Exception(str(data.index(q)) + ") Question has no answers.")

                for a in q["answers"]:
                    if a["text"] == "" and a["image"] == "":
                        raise Exception(str(data.index(q)) + ") Answer's text and image cannot both be empty.")
                    if a["text"] is None or a["image"] is None:
                        raise Exception(str(data.index(q)) + ") Some answer's parameters are null or missing.")

        except Exception as e:
            logging.error(getattr(e, 'message', repr(e)))
            logging.fatal(filename + " is invalid. Aborting.")

logging.info("Parsing successful!")