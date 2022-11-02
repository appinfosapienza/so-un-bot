
import os
parent_dir = os.getcwd()


for i in range(21, 51):
    os.chdir(os.path.join(parent_dir, str(i)))
    open("quest.txt", "w")
    open("correct.txt", "w")
    open("wrong.txt", "w")
    open("wrong 2.txt", "w")