; inverts boolean logic
;(define not (lambda (x) (if x false true)))
(define (not x) (if x false true))

; identity returns self
(define-macro identity (lambda (x) x))

; flips the order of positional arguments
(define (flip func) (lambda (arg1 arg2) (func arg2 arg1)))

; partially applies a function
(define (curry func arg1) (lambda (arg) (apply func (cons arg1 (list arg)))))

; composes two functions
(define (compose f g) (lambda (arg) (f (apply g arg))))

(define zero? (curry = 0))
(define positive? (curry < 0))
(define negative? (curry > 0))
(define (odd? num) (= (mod num 2) 1))
(define (even? num) (= (mod num 2) 0))
(define inc (curry + 1))
(define dec (curry - 1))

(define (sign x)
    (cond   (= x 0) 0
            (> x 0) 1
            (< x 0) -1))

(define (abs x) (if (< x 0) (* -1 x) x))

; right fold
(define (foldr func end lst)
    (if (null? lst)
        end
        (func (first lst) (foldr func end (rest lst)))))

; left fold
(define (foldl func accum lst)
    (if (null? lst)
        accum
        (foldl func (func accum (first lst)) (rest lst))))

(define fold foldl)
(define reduce foldr)

(define (unfold func init pred)
    (if (pred init)
        (cons init '())
        (cons init (unfold func (func init) pred))))

(define (sum . lst) (fold + 0 lst))
(define (product . lst) (fold * 1 lst))
(define (and . lst) (fold && true lst))
(define (or . lst) (fold || false lst))

(define (any? pred sequence) (or (map pred sequence)))
(define (every? pred sequence) (and (map pred sequence)))
(define (map func sequence) (foldr (lambda (x y) (cons (func x) y)) '() sequence))
(define (filter pred sequence) (foldr (lambda (x y) (if (pred x) (cons x y) y)) '() sequence))
(define (max first . rest) (fold (lambda (old new) (if (> old new) old new)) first rest))
(define (min first . rest) (fold (lambda (old new) (if (< old new) old new)) first rest))

(define-macro cond (lambda xs
    (if (> (length xs) 0) 
        (list 'if (first xs)
        (if (> (length xs) 1) 
            (nth xs 1) (throw "Expected even number of arguments"))
            (cons 'cond (rest (rest xs)))))))

(define (number? x) (= (type-of x) 'number))
(define (string? x) (= (type-of x) 'string))
(define (bool? x) (= (type-of x) 'bool))
(define (atom? x) (= (type-of x) 'atom))
(define (keyword? x) (= (type-of x) 'keyword))
(define (symbol? x) (= (type-of x) 'symbol))
(define (list? x) (= (type-of x) 'list))
(define (vector? x) (= (type-of x) 'vector))
(define (hashmap? x) (= (type-of x) 'hashmap))
(define (sequential? x) (or (list? x) (vector? x)))
(define (container? x) (or (list? x) (vector? x) (hashmap? x)))
(define false? (curry = false))
(define true? (curry = true))

(define (file-read-content filepath) 
    (let (p (file-open-read filepath)) 
        (try (do
            (define c (file-read p))
            (file-close p)
            c)
        (catch e (do 
            (file-close p)
            (throw e))
        ))
    ))

(define (file-write-content filepath content) 
    (let (p (file-open-write filepath)) 
        (try (do
            (file-write p content)
            (file-close p))
        (catch e (do 
            (file-close p)
            (throw e))
        ))
    ))

(define (load-file f) (eval (read (strcat "(do " (file-read-content f) "\nnil)"))))

; creates a list from start (inclusive) to stop (not inclusive)
(define (range start stop step)
    (if (= step 0) 
        (throw "step can not be 0")
        (if (|| (&& (> step 0) (< start stop)) (&& (< step 0) (> start stop)))
            (cons start (range (+ start step) stop step)) 
            (list))
    ))
