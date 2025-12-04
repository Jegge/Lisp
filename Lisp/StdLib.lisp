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

; create a list from arguments
(define (list . xs) xs)

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

(define (second xs) (first (rest xs)))

; right fold
(define (foldr func end xs)
    (if (null? xs)
        end
        (func (first xs) (foldr func end (rest xs)))))

; left fold
(define (foldl func accum xs)
    (if (null? xs)
        accum
        (foldl func (func accum (first xs)) (rest xs))))

(define fold foldl)
(define reduce foldr)

(define (unfold func init pred)
    (if (pred init)
        (cons init '())
        (cons init (unfold func (func init) pred))))

(define (sum . xs) (fold + 0 xs))
(define (product . xs) (fold * 1 xs))
(define (and . xs) (fold && true xs))
(define (or . xs) (fold || false xs))

(define (map func xs) (foldr (lambda (x y) (cons (func x) y)) '() xs))

(define (filter pred xs) (foldr (lambda (x y) (if (pred x) (cons x y) y)) '() xs))

(define (zip xs ys) 
    (if (|| (null? xs) (null? ys)) 
        '() 
        (cons (list (first xs) (first ys)) (zip (rest xs) (rest ys)))))

(define (any? pred . xs) (apply or (map pred xs)))
(define (every? pred . xs) (apply and (map pred xs)))
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
    
;(define (range start stop step)
;    (cond (= step 0) (throw "step can not be 0")
;          (> step 0) (unfold (lambda (x) (+ x step)) start (lambda (x) (>= x stop)))
;          (< step 0) (unfold (lambda (x) (- x step)) start (lambda (x) (<= x stop)))
;    ))

(define (quit) (exit 0))