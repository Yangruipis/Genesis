function [f,A]=Dis_ICM(a)
m=unidrnd(12);n=unidrnd(12);
K=a(m,:);
a(m,:)=a(n,:);
a(n,:)=K;
i=2;
M=[26.57	26.57	61.51	61.51	50.44	50.44	50.44	61.51	26.57	25.25	25.25	25.25
]
S=0.1963;
V=2;
t=300*24*60*60;
k1=0.2;
C=500000;
 for i=2:12
D(1,i)=6370.1* acos(sin(a(i,2)*pi/180)* sin(a(i-1,2)* pi/180 )+cos(a(i,2)* pi/180 )*cos(a(i-1,2)* pi/180 )*cos(a(i,1)* pi/180 -a(i-1,1)* pi/180 ) ) *C- S*V*t*k1^(i-1)*(1-k1)*M(i);
end
D(1,1)=6370.1* acos(sin(a(1,2)*pi/180)* sin(34.946646* pi/180 )+cos(a(1,2)* pi/180 )*cos(34.946646* pi/180 )*cos(a(1,1)* pi/180 -114.110931* pi/180 ) ) *C- S*V*t*(1-k1)*M(1);
A=a(:,1);
f=sum(D)
end




function [f,A,F]= minDis_ICM(a)
M = [26.57	26.57	61.51	61.51	50.44	50.44	50.44	61.51	26.57	25.25	25.25	25.25
]
S=0.1963;
V=2;
t=300*24*60*60;
k1=0.2;
C=500000;



 for i=2:12
D(1,i)=6370.1* acos(sin(a(i,2)*pi/180)* sin(a(i-1,2)* pi/180 )+cos(a(i,2)* pi/180 )*cos(a(i-1,2)* pi/180 )*cos(a(i,1)* pi/180 -a(i-1,1)* pi/180 ) ) *C- S*V*t*k1^(i-1)*(1-k1)*M(i);
end
D(1,1)=6370.1* acos(sin(a(1,2)*pi/180)* sin(34.946646* pi/180 )+cos(a(1,2)* pi/180 )*cos(34.946646* pi/180 )*cos(a(1,1)* pi/180 -114.110931* pi/180 ) ) *C- S*V*t*(1-k1)*M(1);

D0=sum(D);
count=0;
T=1000;x=0;k=0.1;
E1=D0;

while T>1
    count=count+1;
  for j=1:10;
  %E1=D0;
  [E2,A]=Dis_ICM(a);
    if E1>=E2
      D0=E2
    else
      r=exp((E1-E2)/(5*10^5*T));
      p=rand();
        if r>p
          [D0,A]=Dis_ICM(a);
        else
          D0=D0;
        end
    end
    F((count-1)*10+j)=D0;
    E1=D0;
   end
T=0.9*T;

end

f=E1
end
