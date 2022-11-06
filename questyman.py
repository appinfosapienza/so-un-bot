# A python helper tool to generate questions with the directory tree format

import os 

prefix = input("Specify a prefix (default is no prefix): ")
separator = input("Specify a separator (default is no separator): ")
current = int(input("Number of the next question: "))
path = input("Specify the path: ")

while True:
    dirname = path + "/" + prefix + separator + str(current)
    os.mkdir(dirname)
    open(dirname + "/quest.txt", 'a').close()
    os.system("\"" + dirname + "/quest.txt" + "\"")
    open(dirname + "/correct.txt", 'a').close()
    os.system("\"" + dirname + "/correct.txt" + "\"")
    open(dirname + "/wrong 1.txt", 'a').close()
    os.system("\"" + dirname + "/wrong 1.txt" + "\"")
    open(dirname + "/wrong 2.txt", 'a').close()
    os.system("\"" + dirname + "/wrong 2.txt" + "\"")

    current += 1