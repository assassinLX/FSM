using UnityEngine;
using System.Collections;

public class FSMController: Behaviour{
 
        public enum State{
         idle,
         eat,
         run,
        }
        public State _state;
 
        public idle _idle;

        public eat _eat;

        public run _run;

 
      private void Start(){
            _state = new State();
            _idle = new idle();
            _eat = new eat();
            _run = new run();
      }

     private void Update(){
     //...放入判断的条件 
        if(false){
              _state =  State.idle;
        }
        else if (false){
               _state =  State.eat;
        }
        else if (false){
               _state =  State.run;
        }


     if(_state == State.idle){
            if(_eat.self != null){
                _eat._Over();
        }
            if(_run.self != null){
                _run._Over();
        }
            if(_idle == null){
                _idle._Start(transform.gameObject);
        }
            _idle._Update();
     }
     if(_state == State.eat){
            if(_idle.self != null){
                _idle._Over();
        }
            if(_run.self != null){
                _run._Over();
        }
            if(_eat == null){
                _eat._Start(transform.gameObject);
        }
            _eat._Update();
     }
     if(_state == State.run){
            if(_idle.self != null){
                _idle._Over();
        }
            if(_eat.self != null){
                _eat._Over();
        }
            if(_run == null){
                _run._Start(transform.gameObject);
        }
            _run._Update();
     }
    }
}
