# A python helper script that finds duplicated questions
# and deletes them.

import os
import shutil

path = input("Specify the path: ")

quests = list(os.walk(path))[0][1]

m = {}

print("Loading questions...")

for q in quests:
    qq = open(path + "/" + q + "/quest.txt", "r").read()
    if qq.startswith("img=") and "\n" in qq:
        qq = qq[qq.index("\n"):]

    cc = open(path + "/" + q + "/correct.txt", "r").read()
    if cc.startswith("img=") and "\n" in cc:
        cc = cc[cc.index("\n"):]
    
    qq = qq + cc

    qq = qq.replace("\n", "")
    qq = qq.replace("<pre>", "")
    qq = qq.replace("<code>", "")
    qq = qq.replace("</pre>", "")
    qq = qq.replace("</code>", "")
    qq = qq.replace("'", "")
    qq = qq.replace("à", "a")
    qq = qq.replace("è", "e")
    qq = qq.replace("ì", "i")
    qq = qq.replace("ò", "o")
    qq = qq.replace("ù", "u")
    qq = qq.replace(" ", "")
    m[q] = qq

print("Comparing questions...")

rev_dict = {}

for key, value in m.items():
    rev_dict.setdefault(value, set()).add(key)


result = [values for key, values in rev_dict.items()
                              if len(values) > 1]

if result:
    print("Duplicate questions:", str(result))
else:
    print("There are no duplicates!")
    exit()

for dup in result:
    #delete the duplicates
    ld = list(dup)
    for i in range(len(ld)):
        if i == 0:
            print("Keeping " + ld[i])
            continue
        print("Deleting", ld[i])
        shutil.rmtree(path + "/" + ld[i])