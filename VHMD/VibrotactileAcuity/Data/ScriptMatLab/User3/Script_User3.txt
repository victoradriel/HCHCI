close all
clear all

N=[0 5 15 25]';
number_hits=[5 9 9 10]';
nobservation=[10 10 10 10]';
data_reshape = [N, number_hits, nobservation]

priors.m_or_a = 'None';
priors.w_or_b = 'None';
priors.lambda = 'Beta(2,20)';
priors.gamma  = '0.5';

figure(1)
B = BootstrapInference(data_reshape, priors, 'nafc', 2, 'sigmoid', 'logistic', ...
    'core', 'mw0.1','verbose','cuts',.75);
GoodnessOfFit(B);
thr75temporalRight = getThres(B,1)


fname=strcat('User3.txt'); 
    
f1=fopen(fname,'w');
fprintf(f1,'%6s\t','FrLeft');
fprintf(f1,'%2.2f\n',thr75temporalRight);
%%

N=[0 5 15 25]';
number_hits=[5 9 10 10]';
nobservation=[10 10 10 10]';
data_reshape = [N, number_hits, nobservation]

priors.m_or_a = 'None';
priors.w_or_b = 'None';
priors.lambda = 'Beta(2,20)';
priors.gamma  = '0.5';

figure(2)
B = BootstrapInference(data_reshape, priors, 'nafc', 2, 'sigmoid', 'logistic', ...
    'core', 'mw0.1','verbose','cuts',.75);
GoodnessOfFit(B);
thr75temporalRight = getThres(B,1)
    
f1=fopen(fname,'a');
fprintf(f1,'%6s\t','FrCLft');
fprintf(f1,'%2.2f\n',thr75temporalRight);
%%

N=[0 5 15 25]';
number_hits=[5 10 10 10]';
nobservation=[10 10 10 10]';
data_reshape = [N, number_hits, nobservation]

priors.m_or_a = 'None';
priors.w_or_b = 'None';
priors.lambda = 'Beta(2,20)';
priors.gamma  = '0.5';

figure(3)
B = BootstrapInference(data_reshape, priors, 'nafc', 2, 'sigmoid', 'logistic', ...
    'core', 'mw0.1','verbose','cuts',.75);
GoodnessOfFit(B);
thr75temporalRight = getThres(B,1)
    
f1=fopen(fname,'a');
fprintf(f1,'%6s\t','FrCRht');
fprintf(f1,'%2.2f\n',thr75temporalRight);
%%

N=[0 5 15 25]';
number_hits=[5 9 10 10]';
nobservation=[10 10 10 10]';
data_reshape = [N, number_hits, nobservation]

priors.m_or_a = 'None';
priors.w_or_b = 'None';
priors.lambda = 'Beta(2,20)';
priors.gamma  = '0.5';

figure(4)
B = BootstrapInference(data_reshape, priors, 'nafc', 2, 'sigmoid', 'logistic', ...
    'core', 'mw0.1','verbose','cuts',.75);
GoodnessOfFit(B);
thr75temporalRight = getThres(B,1)
    
f1=fopen(fname,'a');
fprintf(f1,'%6s\t','FrRght');
fprintf(f1,'%2.2f\n',thr75temporalRight);
%%

N=[0 5 15 25]';
number_hits=[5 6 10 10]';
nobservation=[10 10 10 10]';
data_reshape = [N, number_hits, nobservation]

priors.m_or_a = 'None';
priors.w_or_b = 'None';
priors.lambda = 'Beta(2,20)';
priors.gamma  = '0.5';

figure(5)%change the number
B = BootstrapInference(data_reshape, priors, 'nafc', 2, 'sigmoid', 'logistic', ...
    'core', 'mw0.1','verbose','cuts',.75);
GoodnessOfFit(B);
thr75temporalRight = getThres(B,1)
    
f1=fopen(fname,'a');
fprintf(f1,'%6s\t','TpLeft');
fprintf(f1,'%2.2f\n',thr75temporalRight);
%%

N=[0 5 15 25]';
number_hits=[5 9 9 9]';
nobservation=[10 10 10 10]';
data_reshape = [N, number_hits, nobservation]

priors.m_or_a = 'None';
priors.w_or_b = 'None';
priors.lambda = 'Beta(2,20)';
priors.gamma  = '0.5';

figure(6)%change the number
B = BootstrapInference(data_reshape, priors, 'nafc', 2, 'sigmoid', 'logistic', ...
    'core', 'mw0.1','verbose','cuts',.75);
GoodnessOfFit(B);
thr75temporalRight = getThres(B,1)
    
f1=fopen(fname,'a');
fprintf(f1,'%6s\t','TpCLft');
fprintf(f1,'%2.2f\n',thr75temporalRight);
%%

N=[0 5 15 25]';
number_hits=[5 6 10 10]';
nobservation=[10 10 10 10]';
data_reshape = [N, number_hits, nobservation]

priors.m_or_a = 'None';
priors.w_or_b = 'None';
priors.lambda = 'Beta(2,20)';
priors.gamma  = '0.5';

figure(7)%change the number
B = BootstrapInference(data_reshape, priors, 'nafc', 2, 'sigmoid', 'logistic', ...
    'core', 'mw0.1','verbose','cuts',.75);
GoodnessOfFit(B);
thr75temporalRight = getThres(B,1)
    
f1=fopen(fname,'a');
fprintf(f1,'%6s\t','TpCRht');
fprintf(f1,'%2.2f\n',thr75temporalRight);
%%

N=[0 5 15 25]';
number_hits=[5 9 10 10]';
nobservation=[10 10 10 10]';
data_reshape = [N, number_hits, nobservation]

priors.m_or_a = 'None';
priors.w_or_b = 'None';
priors.lambda = 'Beta(2,20)';
priors.gamma  = '0.5';

figure(8)%change the number
B = BootstrapInference(data_reshape, priors, 'nafc', 2, 'sigmoid', 'logistic', ...
    'core', 'mw0.1','verbose','cuts',.75);
GoodnessOfFit(B);
thr75temporalRight = getThres(B,1)
    
f1=fopen(fname,'a');
fprintf(f1,'%6s\t','TpRght');
fprintf(f1,'%2.2f\n',thr75temporalRight);
%%

N=[0 5 15 25]';
number_hits=[5 6 8 6]';
nobservation=[10 10 10 10]';
data_reshape = [N, number_hits, nobservation]

priors.m_or_a = 'None';
priors.w_or_b = 'None';
priors.lambda = 'Beta(2,20)';
priors.gamma  = '0.5';

figure(9)%change the number
B = BootstrapInference(data_reshape, priors, 'nafc', 2, 'sigmoid', 'logistic', ...
    'core', 'mw0.1','verbose','cuts',.75);
GoodnessOfFit(B);
thr75temporalRight = getThres(B,1)
    
f1=fopen(fname,'a');
fprintf(f1,'%6s\t','OcLeft');
fprintf(f1,'%2.2f\n',thr75temporalRight);
%%

N=[0 5 15 25]';
number_hits=[5 10 10 10]';
nobservation=[10 10 10 10]';
data_reshape = [N, number_hits, nobservation]

priors.m_or_a = 'None';
priors.w_or_b = 'None';
priors.lambda = 'Beta(2,20)';
priors.gamma  = '0.5';

figure(10)%change the number
B = BootstrapInference(data_reshape, priors, 'nafc', 2, 'sigmoid', 'logistic', ...
    'core', 'mw0.1','verbose','cuts',.75);
GoodnessOfFit(B);
thr75temporalRight = getThres(B,1)
    
f1=fopen(fname,'a');
fprintf(f1,'%6s\t','OcCLft');
fprintf(f1,'%2.2f\n',thr75temporalRight);
%%

N=[0 5 15 25]';
number_hits=[5 10 10 10]';
nobservation=[10 10 10 10]';
data_reshape = [N, number_hits, nobservation]

priors.m_or_a = 'None';
priors.w_or_b = 'None';
priors.lambda = 'Beta(2,20)';
priors.gamma  = '0.5';

figure(11)%change the number
B = BootstrapInference(data_reshape, priors, 'nafc', 2, 'sigmoid', 'logistic', ...
    'core', 'mw0.1','verbose','cuts',.75);
GoodnessOfFit(B);
thr75temporalRight = getThres(B,1)
    
f1=fopen(fname,'a');
fprintf(f1,'%6s\t','OcCRht');
fprintf(f1,'%2.2f\n',thr75temporalRight);
%%

N=[0 5 15 25]';
number_hits=[5 5 7 9]';
nobservation=[10 10 10 10]';
data_reshape = [N, number_hits, nobservation]

priors.m_or_a = 'None';
priors.w_or_b = 'None';
priors.lambda = 'Beta(2,20)';
priors.gamma  = '0.5';

figure(12)%change the number
B = BootstrapInference(data_reshape, priors, 'nafc', 2, 'sigmoid', 'logistic', ...
    'core', 'mw0.1','verbose','cuts',.75);
GoodnessOfFit(B);
thr75temporalRight = getThres(B,1)
    
f1=fopen(fname,'a');
fprintf(f1,'%6s\t','OcRght');
fprintf(f1,'%2.2f\n',thr75temporalRight);
%%

N=[0 5 15 25]';
number_hits=[5 9 10 10]';
nobservation=[10 10 10 10]';
data_reshape = [N, number_hits, nobservation]

priors.m_or_a = 'None';
priors.w_or_b = 'None';
priors.lambda = 'Beta(2,20)';
priors.gamma  = '0.5';

figure(13)%change the number
B = BootstrapInference(data_reshape, priors, 'nafc', 2, 'sigmoid', 'logistic', ...
    'core', 'mw0.1','verbose','cuts',.75);
GoodnessOfFit(B);
thr75temporalRight = getThres(B,1)
    
f1=fopen(fname,'a');
fprintf(f1,'%6s\t','FTLeft');
fprintf(f1,'%2.2f\n',thr75temporalRight);
%%

N=[0 5 15 25]';
number_hits=[5 10 10 10]';
nobservation=[10 10 10 10]';
data_reshape = [N, number_hits, nobservation]

priors.m_or_a = 'None';
priors.w_or_b = 'None';
priors.lambda = 'Beta(2,20)';
priors.gamma  = '0.5';

figure(14)%change the number
B = BootstrapInference(data_reshape, priors, 'nafc', 2, 'sigmoid', 'logistic', ...
    'core', 'mw0.1','verbose','cuts',.75);
GoodnessOfFit(B);
thr75temporalRight = getThres(B,1)
    
f1=fopen(fname,'a');
fprintf(f1,'%6s\t','FTCLft');
fprintf(f1,'%2.2f\n',thr75temporalRight);
%%

N=[0 5 15 25]';
number_hits=[5 9 10 9]';
nobservation=[10 10 10 10]';
data_reshape = [N, number_hits, nobservation]

priors.m_or_a = 'None';
priors.w_or_b = 'None';
priors.lambda = 'Beta(2,20)';
priors.gamma  = '0.5';

figure(15)%change the number
B = BootstrapInference(data_reshape, priors, 'nafc', 2, 'sigmoid', 'logistic', ...
    'core', 'mw0.1','verbose','cuts',.75);
GoodnessOfFit(B);
thr75temporalRight = getThres(B,1)
    
f1=fopen(fname,'a');
fprintf(f1,'%6s\t','FTCRht');
fprintf(f1,'%2.2f\n',thr75temporalRight);
%%

N=[0 5 15 25]';
number_hits=[5 6 8 6]';
nobservation=[10 10 10 10]';
data_reshape = [N, number_hits, nobservation]

priors.m_or_a = 'None';
priors.w_or_b = 'None';
priors.lambda = 'Beta(2,20)';
priors.gamma  = '0.5';

figure(16)
B = BootstrapInference(data_reshape, priors, 'nafc', 2, 'sigmoid', 'logistic', ...
    'core', 'mw0.1','verbose','cuts',.75);
GoodnessOfFit(B);
thr75temporalRight = getThres(B,1)
    
f1=fopen(fname,'a');
fprintf(f1,'%6s\t','FTRght');
fprintf(f1,'%2.2f\n',thr75temporalRight);


%%
fclose(f1)