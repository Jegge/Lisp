; inverts boolean logic
(define not (lambda (x) (if x false true)))

; identity returns self
(define-macro identity (lambda (x) x))

; flips the order of positional arguments
(define flip (lambda (func) (lambda (arg1 arg2) (func arg2 arg1))))

; partially applies a function
(define curry (lambda (func arg1) (lambda (arg) (apply func (cons arg1 (list arg))))))

; composes two functions
(define compose (lambda (f g) (lambda (arg) (f (apply g arg)))))

(define zero? (curry = 0))
(define positive? (curry < 0))
(define negative? (curry > 0))
(define odd? (lambda (num) (= (mod num 2) 1)))
(define even? (lambda (num) (= (mod num 2) 0)))
(define inc (curry + 1))
(define dec (curry - 1))

(define sign (lambda (x)
    (cond   (= x 0) 0
            (> x 0) 1
            (< x 0) -1)))

(define abs (lambda (x) (if (< x 0) (* -1 x) x)))

; right fold
(define foldr (lambda (func end lst)
    (if (null? lst)
        end
        (func (first lst) (foldr func end (rest lst))))))

; left fold
(define foldl (lambda (func accum lst)
    (if (null? lst)
        accum
        (foldl func (func accum (first lst)) (rest lst)))))

(define fold foldl)
(define reduce foldr)

(define unfold (lambda (func init pred)
    (if (pred init)
        (cons init '())
        (cons init (unfold func (func init) pred)))))

(define sum (lambda (sequence) (fold + 0 sequence)))
(define product (lambda (sequence) (fold * 1 sequence)))
(define and (lambda (sequence) (fold && true sequence)))
(define or (lambda (sequence) (fold || false sequence)))
(define any? (lambda (pred sequence) (or (map pred sequence))))
(define every? (lambda (pred sequence) (and (map pred sequence))))

(define map (lambda (func sequence) (foldr (lambda (x y) (cons (func x) y)) '() sequence)))
(define filter (lambda (pred sequence) (foldr (lambda (x y) (if (pred x) (cons x y) y)) '() sequence)))

(define-macro cond (lambda (& xs)
    (if (> (count xs) 0) 
        (list 'if (first xs)
        (if (> (count xs) 1) 
            (nth xs 1) (throw "Expected even number of arguments"))
            (cons 'cond (rest (rest xs)))))))

(define number? (lambda (x) (= (type-of x) 'number)))
(define string? (lambda (x) (= (type-of x) 'string)))
(define bool? (lambda (x) (= (type-of x) 'bool)))
(define atom? (lambda (x) (= (type-of x) 'atom)))
(define keyword? (lambda (x) (= (type-of x) 'keyword)))
(define symbol? (lambda (x) (= (type-of x) 'symbol)))
(define list? (lambda (x) (= (type-of x) 'list)))
(define vector? (lambda (x) (= (type-of x) 'vector)))
(define hashmap? (lambda (x) (= (type-of x) 'hashmap)))
(define sequential? (lambda (x) (or (list (list? x) (vector? x)))))
(define container? (lambda (x) (or (list (list? x) (vector? x) (hashmap? x)))))
(define false? (curry = false))
(define true? (curry = true))

(define file-read-content (lambda (filepath) 
    (let (p (file-open-read filepath)) (do
        (define c (file-read p))
        (file-close p)
        c)
    )))  

(define file-write-content (lambda (filepath content) 
    (let (p (file-open-write filepath)) (do
        (file-write p content)
        (file-close p)
        )
    )))

(define load-file (lambda (f) (eval (read (strcat "(do " (file-read-content f) "\nnil)")))))

; creates a list from start (inclusive) to stop (not inclusive)
(define range (lambda (start stop step)
    (if (= step 0) 
        (throw "step can not be 0")
        (if (|| (&& (> step 0) (< start stop)) (&& (< step 0) (> start stop)))
            (cons start (range (+ start step) stop step)) 
            (list))
    )))
