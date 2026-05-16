name=input("Enter your name:")
try:
    age=int(input("Entrt your age:"))
    print(f"Hello {name}, your age is {age}.")
    if age<0:
        print("Invalid age.")
    elif age<18:
        print("You are a minor.")
    else:
        print("You are an adult.")
except:
    print("Invalid age")