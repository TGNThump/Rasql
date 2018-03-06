from pptree import print_tree

def main():
    
    grammar = (
    ("<E>","<E>","+","<E>"),
    ("<E>","(","<E>",")"),
    ("<E>","<N>"),
    ("<N>","0","<N>"),
    ("<N>","1","<N>"),
    ("<N>","0"),
    ("<N>","1")
    )
    
    parser = Parser(grammar)
    i = input()
    print(parser.valid(i))
    if parser.valid(i):
        print_tree(parser.parse(i),childattr='children',nameattr='name')

class Parser:
    
    grammar = ()
    
    def __init__(self, grammar):
        self.grammar = grammar
    
    def parse(self, string):
        orderedStateSets = self.reverseStateSets(self.stateSets(string))
        rootRule = self.grammar[0][0]
        for set in orderedStateSets:
            for item in set:
                item.print()
        root = None
        
        for item in orderedStateSets[0]:
            if item.nonTerminal == rootRule and item.end == len(string):
                root = self.constructNode(item,orderedStateSets)
                break
        return root
        
    def constructNode(self,item,orderedStateSets):
        children = []
        startIndex = item.start
        for symbol in item.symbols:
            if self.terminal(symbol):
                children.append(Node(symbol,[]))
                startIndex += 1
            else:
                for potentialItem in(orderedStateSets[startIndex]):
                    if potentialItem.nonTerminal == symbol:
                        newNode = self.constructNode(potentialItem,orderedStateSets)
                        if newNode.leaves() <= item.end - startIndex:
                            children.append(newNode)
                            startIndex = item.end
                            break       

        return Node(item.nonTerminal,children)
        
    def reverseStateSets(self, stateSets):
        reduced = [[item for item in set if item.fullyParsed()] for set in stateSets]
        reversed =[[] for i in range(len(reduced))]
        for i, set in enumerate(reduced):
            for item in set:
                item.end = i
                reversed[item.start].append(item)
                
        return reversed
    
    def stateSets(self, string):
    
        stateSets = [[] for i in range(len(string)+1)]
        
        for rule in self.grammar:
            if rule[0] == self.grammar[0][0]:
                stateSets[0].append(EarleyItem(rule,0,0))
            
        for i, stateSet in enumerate(stateSets):
            for item in stateSet:
                if item.fullyParsed():
                    stateSet += self.complete(item,stateSets)
                elif self.terminal(item.nextSymbol()):
                    self.scan(item,i,string,stateSets)
                else:
                    self.addUnique(self.predict(item,i),stateSet)
        return stateSets
    
    def valid(self, string):
        valid = False
        stateSets = self.stateSets(string)
        for item in stateSets[-1]:
            if item.fullyParsed() and item.start == 0:
                valid = True
        return valid

    def addUnique(self,elements, list):
        for element in elements:
            shouldAdd = True
            for existingElement in list:
                if existingElement == element:
                    shouldAdd = False
                    break
            if shouldAdd:
                list.append(element)
                    
    def complete(self,item,stateSets):
        stateSet = stateSets[item.start]
        parents = []
        for potentialParent in stateSet:
            if potentialParent.nextSymbol() == item.nonTerminal:
                parents.append(potentialParent.nextItem())
        return parents
                
    def scan(self,item, i, string, stateSets):
        if i<len(string) and string[i] == item.nextSymbol():
            stateSets[i+1].append(item.nextItem())
            
    def predict(self,item,i):
        items = []
        for rule in self.grammar:
            if item.nextSymbol() == rule[0]:
                items.append(EarleyItem(rule,i,0))
        return items
        
    def terminal(self,string):
        for rule in self.grammar:
            if rule[0] == string:
                return False
        return True
        
class Node:
    name = ""
    children = []
    
    def __init__(self, name, children = []):
        self.name = name
        self.children = children
        
    def addChild(self,child):
        self.children.append(child)
        
    def leaves(self):
        total = 0;
        if len(self.children) == 0:
            total = 1
        else:
            
            for child in self.children:
                total += child.leaves()
                
        return total

class EarleyItem:
    parsed = 0
    nonTerminal = ""
    symbols = ()
    start = 0
    end = -1
    backPointer = None
    
    def __init__(self,rule,start,parsed):
        self.nonTerminal = rule[0]
        self.symbols = rule[1:]
        self.start = start
        self.parsed = parsed
        
    def nextSymbol(self):
        if self.fullyParsed():
            return None
        else:
            return self.symbols[self.parsed]
     
    def fullyParsed(self):
        if self.parsed == len(self.symbols):
            return True
        else:
            return False
    
    def nextItem(self):
        return EarleyItem((self.nonTerminal,)+self.symbols,self.start,self.parsed + 1)
        
    def __eq__(self,other):
        if self.parsed == other.parsed and \
           self.nonTerminal == other.nonTerminal and \
           self.symbols == other.symbols and \
           self.start == other.start:
            return True
        else:
            return False
    def print(self):
        print(self.start,self.end,self.parsed,self.nonTerminal,self.symbols)
        
if __name__ == "__main__":
    main()
