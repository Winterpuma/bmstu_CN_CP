%:- use_module(library(dif)).	% Sound inequality
%:- use_module(library(clpfd)).	% Finite domain constraints
%:- use_module(library(clpb)).	% Boolean constraints
%:- use_module(library(chr)).	% Constraint Handling Rules
%:- use_module(library(when)).	% Coroutining
%:- use_module(library(apply)).
%:- use_module(library(thread)).


:- use_module(library(http/thread_httpd)).
:- use_module(library(http/http_dispatch)).
:- use_module(library(http/http_error)).
:- use_module(library(http/http_json)).
:- use_module(library(http/http_server)).
:- use_module(library(http/http_parameters)).

:- initialization server.

server() :-
	http_server(http_dispatch, [port(8080)]).



:- http_handler(/, queen, [method(_),methods([get,post]),time_limit(6000)]).
queen(_R):-
	myQuery(Ans),
	reply_json(json{answer:Ans}).

queen(_R):-
	format('Content-type: text/plain~n~nNoAnswer', []).

myQuery(Ans) :-
	Ans = 3.


% http://localhost:8080/fact/150000
:- http_handler(root(fact/N), getFact(M, N), [method(M),methods([get])]).


getFact(get, AtomN, R) :-
	atom_number(AtomN, N),
	factorial(N, Ans),
	reply_json(json{answer:Ans}).

factorial(N, -1) :- N < 0, !. % error
factorial(0, 1) :- !.
factorial(N, Res) :- 
	zero_out_state(N),
	factorial(N, 1, Res).

factorial(1, Res, Res) :-  
	update_state, !.
factorial(N, Cur, Res) :- 
	update_state,
	NewN is N - 1,
	NewMult is Cur * N,
	factorial(NewN, NewMult, Res).


:- http_handler(root(state), getState, []).

getState(_) :-
	curTasks(Cur),
	totalTasks(Total),
	reply_json(json{name: swipl, current: Cur, total:Total}).

:- dynamic curTasks/1.
:- dynamic totalTasks/1.
curTasks(0).
totalTasks(0).

zero_out_state(TotalTasks) :-
	curTasks(A),
	retract(curTasks(A)),
	asserta(curTasks(0)),
	totalTasks(B),
	retract(totalTasks(B)),
	asserta(totalTasks(TotalTasks)).

update_state :-
	curTasks(A),
	retract(curTasks(A)),
	NewA is A + 1,
	asserta(curTasks(NewA)).
