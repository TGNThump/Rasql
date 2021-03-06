from pptree import print_tree

def main():
    
    grammar = (
    ("<E>","<E>","**","<E>"),
    ("<E>","<E>","*","<E>"),
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
        orderedStateSets = self.reverseStateSets(self.fullyParsed(self.stateSets(string)))
        rootRule = self.grammar[0][0]
        
        self.printStateSets(orderedStateSets)
        
        root = None
        
        for item in orderedStateSets[0]:
            if item.nonTerminal == rootRule and item.end == len(string):
                root = self.constructTree(item,orderedStateSets)
                break
        return root
        
        
    def constructTree(self,item,stateSets,visited = []):
        print("Item: ",end = "")
        item.print()
    
        visited.append(item)
        children = []
        parent = {}
        depth = item.start
        symbolIndex = 0
        stack = [(set,0) for set in stateSets[depth]] 
        
        while depth != item.end:
            symbol = item.symbols[symbolIndex]
            
            if self.terminal(symbol) and len(symbol) + depth <= item.end:
                print(symbol,depth)
                depth += len(symbol)
                symbolIndex += 1
                if symbolIndex < len(item.symbols) and self.terminal(item.symbols[symbolIndex]):
                        parent[(symbolIndex,depth)] = (symbolIndex-1,depth-len(symbol))
                if depth < item.end:
                    for i in stateSets[depth]:
                        parent[i] = (symbolIndex-1,depth-len(symbol))
                        stack.append((i,symbolIndex))
                elif depth == item.end:
                    children.append((symbolIndex-1,depth-len(symbol)))
            else:
                print(symbol,depth)
                next,symbolIndex = stack.pop()
                depth = next.start
                symbol = item.symbols[symbolIndex]                    
                if next not in visited and next.end <= item.end and next.nonTerminal == symbol:
                    visited.append(next)
                    depth = next.end
                    symbolIndex +=1
                    if  symbolIndex < len(item.symbols) and self.terminal(item.symbols[symbolIndex]):
                        parent[(symbolIndex,depth)] = next
                    if depth < item.end:
                        for i in stateSets[depth]:
                            parent[i] = next
                            stack.append((i,symbolIndex))
                    elif depth == item.end:
                        children.append(next)
        
        
        
        for child in children:
            try:
                children.append(parent[child])
            except:
                pass
       
        
        l = [self.constructTree(child,stateSets,visited) if isinstance(child, EarleyItem) else Node(item.symbols[child[0]]) for child in children]
        
        [print(i) for i in l]
        
        return Node(item.nonTerminal,l)
    
    def sortStates(self,stateSets):
        return None
        #Completely broken do not use
        for set in stateSets:
            for i in range(len(set)-1):
                for j in range(len(set)-1):
                    if self.grammar.index(set[i].rule()) > self.grammar.index(set[i+1].rule()):
                        set[i],set[i+1] = set[i+1],set[i]
                        
        return stateSets
    
    def reverseStateSets(self, stateSets):
        reversed =[[] for i in range(len(stateSets))]
        for i, set in enumerate(stateSets):
            for item in set:
                item.end = i
                reversed[item.start].append(item)
                
        return reversed
        
    def fullyParsed(self,stateSets):
        return [[item for item in set if item.fullyParsed()] for set in stateSets]
    
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
        if i+len(item.nextSymbol())-1<len(string) and string[i:i+len(item.nextSymbol())] == item.nextSymbol():
            stateSets[i+len(item.nextSymbol())].append(item.nextItem())
            
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
        
    def printStateSets(self, sets):
        for i, set in enumerate(sets):
            print("===",i,"===")
            self.printStateSet(set)
            print()
        print()
    
    def printStateSet(self,set):
        for item in set:
            item.print()
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
    
    def __hash__(self):
        return hash((self.nonTerminal, self.symbols))
        
    def rule(self):
        return (self.nonTerminal,)+self.symbols
    
    def print(self):
        print(self.start,self.end,self.parsed,self.nonTerminal,self.symbols)
        
if __name__ == "__main__":
    main()
